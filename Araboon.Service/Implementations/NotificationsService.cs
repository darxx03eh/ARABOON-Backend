using Araboon.Data.Entities;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository notificationsRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly IEmailService emailService;

        public NotificationsService(INotificationsRepository notificationsRepository, IMangaRepository mangaRepository, IEmailService emailService)
        {
            this.notificationsRepository = notificationsRepository;
            this.mangaRepository = mangaRepository;
            this.emailService = emailService;
        }
        public async Task<string> AddToNotificationsAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = notificationsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "NotificationsServiceforRegisteredUsersOnly";
                var exist = await notificationsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourNotificationsList";
                await notificationsRepository.AddAsync(new Notifications()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "AddedToNotifications";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToNotifications";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(int pageNumber, int pageSize)
        {
            var flag = await notificationsRepository.IsAdmin();
            var (message, mangas) = await notificationsRepository.GetPaginatedNotificationsMangaAsync(pageNumber, pageSize, flag);
            return message switch
            {
                "NotificationsServiceforRegisteredUsersOnly" => ("NotificationsServiceforRegisteredUsersOnly", null),
                "ThereAreNoMangaInYourNotificationsList" => ("ThereAreNoMangaInYourNotificationsList", null),
                "TheMangaWasFoundInYourNotificationsList" => ("TheMangaWasFoundInYourNotificationsList", mangas),
                _ => ("ThereAreNoMangaInYourNotificationsList", null)
            };
        }

        public async Task<string> RemoveFromNotificationsAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = notificationsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "NotificationsServiceforRegisteredUsersOnly";
                var exist = await notificationsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourNotificationsList";
                await notificationsRepository.DeleteAsync(new Notifications()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "RemovedFromNotifications";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromNotifications";
            }
        }

        public async Task SendNotificationsAsync(
            string mangaName,
            int chapterNo,
            string chapterTitle,
            string lang,
            string link,
            IList<(string Name, string Email)> data
        )
        {
            foreach(var d in data)
            {
                await emailService.SendNotificationsEmailsAsync(
                    d.Name,
                    mangaName,
                    chapterNo,
                    chapterTitle,
                    lang,
                    link,
                    d.Email
                );
            }
        }
    }
}
