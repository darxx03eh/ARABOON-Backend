using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class CurrentlyReadingRepository : GenericRepository<CurrentlyReading>, ICurrentlyReadingRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public CurrentlyReadingRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }
        public async Task<(string, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            string? userId = ExtractUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
                return ("CurrentlyReadingServiceforRegisteredUsersOnly", null);
            IList<int> favoriteMangaIds = new List<int>();
            favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userId))
                               .Select(f => f.MangaID).ToListAsync();
            var currentlyReadingsManga = GetTableNoTracking().Where(c => c.UserID.Equals(int.Parse(userId)))
                                 .OrderByDescending(c => c.Manga.Rate).AsQueryable();
            if (currentlyReadingsManga is null)
                return ("ThereAreNoMangaInYourCurrentlyReadingList", null);
            var mangas = await currentlyReadingsManga.Where(c => isAdmin ? true:c.Manga.IsActive)
                .Select(c => new GetPaginatedCurrentlyReadingsMangaResponse()
            {
                MangaID = c.MangaID,
                MangaName = TransableEntity.GetTransable(c.Manga.MangaNameEn, c.Manga.MangaNameAr),
                MangaImageUrl = c.Manga.MainImage,
                IsFavorite = favoriteMangaIds.Contains(c.MangaID),
                AuthorName = TransableEntity.GetTransable(c.Manga.AuthorEn, c.Manga.AuthorAr),
                LastChapter = c.Manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                .Select(chapter => new LastChapter()
                {
                    ChapterNo = chapter.ChapterNo,
                    ChapterID = chapter.ChapterID,
                    Views = chapter.ReadersCount
                }).FirstOrDefault()
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (mangas.Data.Count().Equals(0))
                return ("ThereAreNoMangaInYourCurrentlyReadingList", null);
            return ("TheMangaWasFoundInYourCurrentlyReadingList", mangas);
        }
        public async Task<bool> IsMangaExistForUser(int mangaId, int userId)
        {
            var manga = context.CurrentlyReadings.Any(
                c => c.MangaID.Equals(mangaId) && c.UserID.Equals(userId)
                );
            return manga;
        }
    }
}
