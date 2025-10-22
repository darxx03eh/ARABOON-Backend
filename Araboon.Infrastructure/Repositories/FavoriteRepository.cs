using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Araboon.Infrastructure.Repositories
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public FavoriteRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<(string, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            string? userId = ExtractUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
                return ("FavoritesServiceforRegisteredUsersOnly", null);
            var favoritesManga = GetTableNoTracking().Where(f => f.UserID.Equals(int.Parse(userId)))
                                 .OrderByDescending(f => f.Manga.Rate).AsQueryable();
            if (favoritesManga is null)
                return ("ThereAreNoMangaInYourFavoritesList", null);
            var mangas = await favoritesManga.Where(f => isAdmin ? true:f.Manga.IsActive)
                .Select(f => new GetPaginatedFavoritesMangaResponse()
            {
                MangaID = f.MangaID,
                MangaName = TransableEntity.GetTransable(f.Manga.MangaNameEn, f.Manga.MangaNameAr),
                AuthorName = TransableEntity.GetTransable(f.Manga.AuthorEn, f.Manga.AuthorAr),
                MangaImageUrl = f.Manga.MainImage,
                IsFavorite = true,
                LastChapter = f.Manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                .Select(chapter => new LastChapter()
                {
                    ChapterNo = chapter.ChapterNo,
                    ChapterID = chapter.ChapterID,
                    Views = chapter.ReadersCount
                }).FirstOrDefault()
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if(mangas.Data.Count().Equals(0))
                return ("ThereAreNoMangaInYourFavoritesList", null);
            return ("TheMangaWasFoundInYourFavoritesList", mangas);
        }
        public async Task<bool> IsMangaExistForUser(int mangaId, int userId)
        {
            var manga = context.Favorites.Any(
                f => f.MangaID.Equals(mangaId) && f.UserID.Equals(userId)
                );
            return manga;
        }
    }
}
