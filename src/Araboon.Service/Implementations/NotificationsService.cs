using Araboon.Data.Entities;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository notificationsRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly IEmailService emailService;
        private readonly ILogger<NotificationsService> logger;

        public NotificationsService(
            INotificationsRepository notificationsRepository,
            IMangaRepository mangaRepository,
            IEmailService emailService,
            ILogger<NotificationsService> logger)
        {
            this.notificationsRepository = notificationsRepository;
            this.mangaRepository = mangaRepository;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task<string> AddToNotificationsAsync(int mangaId)
        {
            logger.LogInformation("Adding manga to notifications - إضافة مانجا للإشعارات | MangaId: {MangaId}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - لم يتم العثور على المانجا | MangaId: {MangaId}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = notificationsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                    return "NotificationsServiceforRegisteredUsersOnly";
                }

                var exist = await notificationsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                {
                    logger.LogInformation("Manga already in notifications list - المانجا موجودة مسبقًا | MangaId: {MangaId}", mangaId);
                    return "ThisMangaIsAlreadyInYourNotificationsList";
                }

                await notificationsRepository.AddAsync(new Notifications()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Manga added to notifications - تمت الإضافة بنجاح | MangaId: {MangaId}", mangaId);
                return "AddedToNotifications";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error adding to notifications - خطأ أثناء الإضافة | MangaId: {MangaId}", mangaId);
                return "ThereWasAProblemAddingToNotifications";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(int pageNumber, int pageSize)
        {
            logger.LogInformation(
                "Getting paginated notifications mangas - جلب المانجات للإشعارات مع صفحات | Page: {Page}, Size: {Size}",
                pageNumber, pageSize);

            var flag = await notificationsRepository.IsAdmin();
            var (message, mangas) = await notificationsRepository
                .GetPaginatedNotificationsMangaAsync(pageNumber, pageSize, flag);

            logger.LogInformation("Result of GetPaginatedNotificationsMangaAsync | Message: {Message}", message);

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
            logger.LogInformation("Removing manga from notifications - إزالة مانجا من الإشعارات | MangaId: {MangaId}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - لم يتم العثور على المانجا | MangaId: {MangaId}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = notificationsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                    return "NotificationsServiceforRegisteredUsersOnly";
                }

                var exist = await notificationsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                {
                    logger.LogInformation("Manga not in notifications list - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                    return "ThisMangaIsNotInYourNotificationsList";
                }

                await notificationsRepository.DeleteAsync(new Notifications()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Manga removed from notifications - تمت الإزالة بنجاح | MangaId: {MangaId}", mangaId);
                return "RemovedFromNotifications";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error removing from notifications - خطأ أثناء الإزالة | MangaId: {MangaId}", mangaId);
                return "ThereWasAProblemDeletingFromNotifications";
            }
        }

        public async Task SendNotificationsAsync(
            string mangaName,
            int chapterNo,
            string chapterTitle,
            string lang,
            string link,
            IList<(string Name, string Email)> data)
        {
            logger.LogInformation(
                "Sending notifications emails - إرسال الإشعارات عبر البريد | Manga: {Manga}, Chapter: {Chapter}",
                mangaName, chapterNo);

            foreach (var d in data)
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

            logger.LogInformation("All notifications emails sent - تم إرسال جميع الإشعارات");
        }
    }
}