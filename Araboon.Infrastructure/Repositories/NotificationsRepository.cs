using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class NotificationsRepository : GenericRepository<Notifications>, INotificationsRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        public NotificationsRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<(String, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            String? userId = ExtractUserIdFromToken();
            if (String.IsNullOrEmpty(userId))
                return ("NotificationsServiceforRegisteredUsersOnly", null);
            IList<Int32> favoriteMangaIds = new List<Int32>();
            favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userId))
                               .Select(f => f.MangaID).ToListAsync();
            var notificationsManga = GetTableNoTracking().Where(c => c.UserID.Equals(Int32.Parse(userId)))
                                 .OrderByDescending(c => c.Manga.Rate).AsQueryable();
            if (notificationsManga is null)
                return ("ThereAreNoMangaInYourNotificationsList", null);
            var mangas = await notificationsManga.Select(c => new GetPaginatedNotificationsMangaResponse()
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
                return ("ThereAreNoMangaInYourNotificationsList", null);
            return ("TheMangaWasFoundInYourNotificationsList", mangas);
        }
        public async Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId)
        {
            var manga = context.Notifications.Any(
                n => n.MangaID.Equals(mangaId) && n.UserID.Equals(userId)
                );
            return manga;
        }
    }
}
