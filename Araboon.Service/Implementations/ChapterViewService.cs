using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class ChapterViewService : IChapterViewService
    {
        private readonly IChapterViewRepository chapterViewRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly ILogger<ChapterViewService> logger;
        private readonly UserManager<AraboonUser> userManager;

        public ChapterViewService(
            IChapterViewRepository chapterViewRepository,
            IMangaRepository mangaRepository,
            ILogger<ChapterViewService> logger,
            UserManager<AraboonUser> userManager)
        {
            this.chapterViewRepository = chapterViewRepository;
            this.mangaRepository = mangaRepository;
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<string> MarkAsReadAsync(int mangaId, int chapterId)
        {
            logger.LogInformation("Marking chapter as read - وضع الفصل كمقروء | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapterId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = chapterViewRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("Mark as read denied for guests - لا يمكن للزوار وضع الفصل كمقروء");
                    return "MarkAsReadForChaptersServiceforRegisteredUsersOnly";
                }

                var exist = await chapterViewRepository.IsMangaAndChapterExistForUser(mangaId, chapterId, int.Parse(userId));
                if (exist)
                {
                    logger.LogInformation("Chapter already marked as read - الفصل محدد كمقروء مسبقًا | MangaId: {MangaId}, ChapterId: {ChapterId}, UserId: {UserId}", mangaId, chapterId, userId);
                    return "ThisChapterInThisMangaIsAlreadyMarkedAsRead";
                }

                var chapterExistInManga = await chapterViewRepository.IsChapterExistInManga(chapterId, mangaId);
                if (!chapterExistInManga)
                {
                    logger.LogWarning("Chapter does not belong to this manga - الفصل لا ينتمي لهذه المانجا | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapterId);
                    return "ThisChapterIsNotInThisManga";
                }

                var user = await userManager.FindByIdAsync(userId);
                var role = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                if (user is not null && role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning("Admins cannot mark chapters as read - لا يمكن للمسؤولين وضع الفصول كمقروءة | MangaId: {MangaId}, ChapterId: {ChapterId}, UserId: {UserId}", mangaId, chapterId, userId);
                    return "AdminsCannotMarkChaptersAsRead";
                }

                await chapterViewRepository.AddAsync(new ChapterView()
                {
                    MangaID = mangaId,
                    ChapterID = chapterId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Chapter marked as read successfully - تم وضع الفصل كمقروء بنجاح | MangaId: {MangaId}, ChapterId: {ChapterId}, UserId: {UserId}", mangaId, chapterId, userId);
                return "MarkedAsRead";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error marking chapter as read - خطأ أثناء وضع الفصل كمقروء | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapterId);
                return "ThereWasAProblemMarkedAsRead";
            }
        }

        public async Task<string> MarkAsUnReadAsync(int mangaId, int chapterId)
        {
            logger.LogInformation("Marking chapter as unread - إزالة علامة مقروء | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapterId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = chapterViewRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("Mark as unread denied for guests - لا يمكن للزوار إزالة علامة المقروء");
                    return "MarkAsUnReadForChaptersServiceforRegisteredUsersOnly";
                }

                var exist = await chapterViewRepository.IsMangaAndChapterExistForUser(mangaId, chapterId, int.Parse(userId));
                if (!exist)
                {
                    logger.LogWarning("Cannot unmark as unread because it's not marked read - لا يمكن إزالة علامة المقروء لأنه غير محدد كمقروء | MangaId: {MangaId}, ChapterId: {ChapterId}, UserId: {UserId}", mangaId, chapterId, userId);
                    return "ThisChapterForThisMangaIsNotExistInMarkedAsRead";
                }

                var user = await userManager.FindByIdAsync(userId);
                var role = (await userManager.GetRolesAsync(user)).FirstOrDefault();
                if (user is not null && role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning("Admins cannot unmark chapters as read - لا يمكن للمسؤولين إزالة علامة المقروء | MangaId: {MangaId}, ChapterId: {ChapterId}, UserId: {UserId}", mangaId, chapterId, userId);
                    return "AdminsCannotUnMarkChaptersAsRead";
                }

                await chapterViewRepository.DeleteAsync(new ChapterView()
                {
                    MangaID = mangaId,
                    ChapterID = chapterId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Chapter marked as unread successfully - تم إزالة علامة المقروء بنجاح | MangaId: {MangaId}, ChapterId: {ChapterId}, UserId: {UserId}", mangaId, chapterId, userId);
                return "MarkedAsUnRead";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error marking chapter as unread - خطأ أثناء إزالة علامة المقروء | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapterId);
                return "ThereWasAProblemMarkedAsUnRead";
            }
        }
    }
}