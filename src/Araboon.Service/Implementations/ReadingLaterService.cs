using Araboon.Data.Entities;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class ReadingLaterService : IReadingLaterService
    {
        private readonly IReadingLaterRepository readingLaterRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly ILogger<ReadingLaterService> logger;

        public ReadingLaterService(
            IReadingLaterRepository readingLaterRepository,
            IMangaRepository mangaRepository,
            ILogger<ReadingLaterService> logger)
        {
            this.readingLaterRepository = readingLaterRepository;
            this.mangaRepository = mangaRepository;
            this.logger = logger;
        }

        public async Task<string> AddToReadingLaterAsync(int mangaId)
        {
            logger.LogInformation("Add to Reading Later started - إضافة مانجا للاحقا | MangaId: {MangaId}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = readingLaterRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                    return "ReadingLaterServiceforRegisteredUsersOnly";
                }

                var exist = await readingLaterRepository.IsMangaExistForUser(mangaId, int.Parse(userId));

                if (exist)
                {
                    logger.LogInformation("Manga already in Reading Later - المانجا موجودة مسبقا في لائحة لاحقا");
                    return "ThisMangaIsAlreadyInYourReadingLaterList";
                }

                await readingLaterRepository.AddAsync(new ReadingLater()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Added to Reading Later successfully - تمت الإضافة بنجاح");
                return "AddedToReadingLater";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding to Reading Later - خطأ في الإضافة");
                return "ThereWasAProblemAddingToReadingLater";
            }
        }

        public async Task<string> RemoveFromReadingLaterAsync(int mangaId)
        {
            logger.LogInformation("Remove from Reading Later started - إزالة مانجا من لاحقا | MangaId: {MangaId}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = readingLaterRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                    return "ReadingLaterServiceforRegisteredUsersOnly";
                }

                var exist = await readingLaterRepository.IsMangaExistForUser(mangaId, int.Parse(userId));

                if (!exist)
                {
                    logger.LogInformation("Manga not in Reading Later list - المانجا ليست في لائحة لاحقا");
                    return "ThisMangaIsNotInYourReadingLaterList";
                }

                await readingLaterRepository.DeleteAsync(new ReadingLater()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Removed from Reading Later successfully - تمت الإزالة بنجاح");
                return "RemovedFromReadingLater";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing from Reading Later - خطأ أثناء الإزالة");
                return "ThereWasAProblemDeletingFromReadingLater";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching paginated Reading Later list - جلب قائمة لاحقا بترتيب صفحات | Page: {Page}, Size: {Size}",
                pageNumber, pageSize);

            bool flag = await readingLaterRepository.IsAdmin();
            var (message, mangas) = await readingLaterRepository.GetPaginatedReadingLaterMangaAsync(pageNumber, pageSize, flag);

            logger.LogInformation("Result: {Message}", message);

            return message switch
            {
                "ReadingLaterServiceforRegisteredUsersOnly" => ("ReadingLaterServiceforRegisteredUsersOnly", null),
                "ThereAreNoMangaInYourReadingLaterList" => ("ThereAreNoMangaInYourReadingLaterList", null),
                "TheMangaWasFoundInYourReadingLaterList" => ("TheMangaWasFoundInYourReadingLaterList", mangas),
                _ => ("ThereAreNoMangaInYourReadingLaterList", null)
            };
        }
    }
}