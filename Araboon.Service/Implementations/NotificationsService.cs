using Araboon.Data.Entities;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Infrastructure.Repositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository notificationsRepository;
        private readonly IMangaRepository mangaRepository;

        public NotificationsService(INotificationsRepository notificationsRepository, IMangaRepository mangaRepository)
        {
            this.notificationsRepository = notificationsRepository;
            this.mangaRepository = mangaRepository;
        }
        public async Task<String> AddToNotificationsAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = notificationsRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "NotificationsServiceforRegisteredUsersOnly";
                var exist = await notificationsRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourNotificationsList";
                await notificationsRepository.AddAsync(new Notifications()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "AddedToNotifications";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToNotifications";
            }
        }

        public async Task<(String, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            var (message, mangas) = await notificationsRepository.GetPaginatedNotificationsMangaAsync(pageNumber, pageSize);
            return message switch
            {
                "NotificationsServiceforRegisteredUsersOnly" => ("NotificationsServiceforRegisteredUsersOnly", null),
                "ThereAreNoMangaInYourNotificationsList" => ("ThereAreNoMangaInYourNotificationsList", null),
                "TheMangaWasFoundInYourNotificationsList" => ("TheMangaWasFoundInYourNotificationsList", mangas),
                _ => ("ThereAreNoMangaInYourNotificationsList", null)
            };
        }

        public async Task<String> RemoveFromNotificationsAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = notificationsRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "NotificationsServiceforRegisteredUsersOnly";
                var exist = await notificationsRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourNotificationsList";
                await notificationsRepository.DeleteAsync(new Notifications()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "RemovedFromNotifications";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromNotifications";
            }
        }
    }
}
