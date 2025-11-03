using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class NotificationsRepository : GenericRepository<Notifications>, INotificationsRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public NotificationsRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<IList<(string Name, string Email)>> GetEmailsToNewChapterNotify(int mangaId)
        {
            var data = await GetTableNoTracking()
                       .Where(notify => notify.MangaID.Equals(mangaId))
                       .Select(notify => new EmailDTO()
                       {
                           Name = $"{notify.User.FirstName} {notify.User.LastName}",
                           Email = notify.User.Email
                       }).ToListAsync();

            return data.Select(x => (x.Name, x.Email)).ToList();
        }

        public async Task<(string, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            string? userId = ExtractUserIdFromToken();
            if (string.IsNullOrEmpty(userId))
                return ("NotificationsServiceforRegisteredUsersOnly", null);
            IList<int> favoriteMangaIds = new List<int>();
            favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userId))
                               .Select(f => f.MangaID).ToListAsync();
            var notificationsManga = GetTableNoTracking().Where(c => c.UserID.Equals(int.Parse(userId)))
                                 .OrderByDescending(c => c.Manga.Rate).AsQueryable();
            if (notificationsManga is null)
                return ("ThereAreNoMangaInYourNotificationsList", null);
            var mangas = await notificationsManga.Where(c => isAdmin ? true:c.Manga.IsActive)
                .Select(c => new GetPaginatedNotificationsMangaResponse()
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
        public async Task<bool> IsMangaExistForUser(int mangaId, int userId)
        {
            var manga = context.Notifications.Any(
                n => n.MangaID.Equals(mangaId) && n.UserID.Equals(userId)
                );
            return manga;
        }
    }
    public class EmailDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
