using Araboon.Data.Entities;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;

namespace Araboon.Infrastructure.Repositories
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FavoriteRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(String, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            String? userId = ExtractUserIdFromToken();
            if (String.IsNullOrEmpty(userId))
                return ("FavoritesServiceforRegisteredUsersOnly", null);
            var favoritesManga = GetTableNoTracking().Where(f => f.UserID.Equals(Int32.Parse(userId)))
                                 .OrderByDescending(f => f.Manga.Rate).AsQueryable();
            if (favoritesManga is null)
                return ("ThereAreNoMangaInYourFavoritesList", null);
            var mangas = await favoritesManga.Select(f => new GetPaginatedFavoritesMangaResponse()
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
        public async Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId)
        {
            var manga = context.Favorites.Any(
                f => f.MangaID.Equals(mangaId) && f.UserID.Equals(userId)
                );
            return manga;
        }
    }
}
