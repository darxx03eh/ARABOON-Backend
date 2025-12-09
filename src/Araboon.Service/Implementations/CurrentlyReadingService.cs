using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class CurrentlyReadingService : ICurrentlyReadingService
    {
        private readonly ICurrentlyReadingRepository currentlyReadingRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly ILogger<CurrentlyReadingService> logger;

        public CurrentlyReadingService(ICurrentlyReadingRepository currentlyReadingRepository, IMangaRepository mangaRepository, ILogger<CurrentlyReadingService> logger)
        {
            this.currentlyReadingRepository = currentlyReadingRepository;
            this.mangaRepository = mangaRepository;
            this.logger = logger;
        }

        public async Task<string> AddToCurrentlyReadingAsync(int mangaId)
        {
            logger.LogInformation("Adding manga to currently reading - إضافة مانجا إلى القائمة الحالية | MangaId: {Id}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {Id}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = currentlyReadingRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                    return "CurrentlyReadingServiceforRegisteredUsersOnly";
                }

                var exist = await currentlyReadingRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                {
                    logger.LogInformation("Manga already in currently reading list - المانجا موجودة مسبقًا | MangaId: {Id}, UserId: {User}", mangaId, userId);
                    return "ThisMangaIsAlreadyInYourCurrentlyReadingList";
                }

                await currentlyReadingRepository.AddAsync(new CurrentlyReading()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Added to currently reading - تمت الإضافة إلى قائمة القراءة الحالية | MangaId: {Id}, UserId: {User}", mangaId, userId);
                return "AddedToCurrentlyReading";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding to currently reading - خطأ أثناء الإضافة | MangaId: {Id}", mangaId);
                return "ThereWasAProblemAddingToCurrentlyReading";
            }
        }

        public async Task<string> RemoveFromCurrentlyReadingAsync(int mangaId)
        {
            logger.LogInformation("Removing manga from currently reading - إزالة مانجا من القراءة الحالية | MangaId: {Id}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {Id}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = currentlyReadingRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                    return "CurrentlyReadingServiceforRegisteredUsersOnly";
                }

                var exist = await currentlyReadingRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                {
                    logger.LogInformation("Manga not in currently reading list - غير موجود في قائمة القراءة الحالية | MangaId: {Id}, UserId: {User}", mangaId, userId);
                    return "ThisMangaIsNotInYourCurrentlyReadingList";
                }

                await currentlyReadingRepository.DeleteAsync(new CurrentlyReading()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Removed from currently reading - تمت الإزالة من القراءة الحالية | MangaId: {Id}, UserId: {User}", mangaId, userId);
                return "RemovedFromCurrentlyReading";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing from currently reading - خطأ أثناء الحذف | MangaId: {Id}", mangaId);
                return "ThereWasAProblemDeletingFromCurrentlyReading";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching paginated currently reading list - جلب قائمة القراءة الحالية مع صفحات | Page: {Page}, Size: {Size}", pageNumber, pageSize);

            var flag = await currentlyReadingRepository.IsAdmin();

            var (message, mangas) =
                await currentlyReadingRepository.GetPaginatedCurrentlyReadingsMangaAsync(pageNumber, pageSize, flag);

            return message switch
            {
                "CurrentlyReadingServiceforRegisteredUsersOnly" => ("CurrentlyReadingServiceforRegisteredUsersOnly", null),
                "ThereAreNoMangaInYourCurrentlyReadingList" => ("ThereAreNoMangaInYourCurrentlyReadingList", null),
                "TheMangaWasFoundInYourCurrentlyReadingList" => ("TheMangaWasFoundInYourCurrentlyReadingList", mangas),
                _ => ("ThereAreNoMangaInYourCurrentlyReadingList", null)
            };
        }
    }
}