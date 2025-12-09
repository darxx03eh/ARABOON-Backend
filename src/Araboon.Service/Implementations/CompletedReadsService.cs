using Araboon.Data.Entities;
using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class CompletedReadsService : ICompletedReadsService
    {
        private readonly ICompletedReadsRepository completedReadsRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly ILogger<CompletedReadsService> logger;

        public CompletedReadsService(ICompletedReadsRepository completedReadsRepository, IMangaRepository mangaRepository, ILogger<CompletedReadsService> logger)
        {
            this.completedReadsRepository = completedReadsRepository;
            this.mangaRepository = mangaRepository;
            this.logger = logger;
        }

        public async Task<string> AddToCompletedReadsAsync(int mangaId)
        {
            logger.LogInformation("Adding manga to completed reads - إضافة مانجا لقائمة المكتمل | MangaId: {Id}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {Id}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = completedReadsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                    return "CompletedReadsServiceforRegisteredUsersOnly";
                }

                var exist = await completedReadsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                {
                    logger.LogInformation("Manga already exists in completed list - المانجا موجودة مسبقًا | MangaId: {Id}, UserId: {UserId}", mangaId, userId);
                    return "ThisMangaIsAlreadyInYourCompletedReadsList";
                }

                await completedReadsRepository.AddAsync(new CompletedReads()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Manga added to completed reads - تم إضافة المانجا للمكتمل | MangaId: {Id}, UserId: {UserId}", mangaId, userId);
                return "AddedToCompletedReads";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding manga to completed reads - خطأ أثناء إضافة المانجا للمكتمل | MangaId: {Id}", mangaId);
                return "ThereWasAProblemAddingToCompletedReads";
            }
        }

        public async Task<string> RemoveFromCompletedReadsAsync(int mangaId)
        {
            logger.LogInformation("Removing manga from completed reads - إزالة مانجا من المكتمل | MangaId: {Id}", mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {Id}", mangaId);
                return "MangaNotFound";
            }

            try
            {
                var userId = completedReadsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                    return "CompletedReadsServiceforRegisteredUsersOnly";
                }

                var exist = await completedReadsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                {
                    logger.LogInformation("Manga not in completed list - المانجا ليست في المكتمل | MangaId: {Id}, UserId: {UserId}", mangaId, userId);
                    return "ThisMangaIsNotInYourCompletedReadsList";
                }

                await completedReadsRepository.DeleteAsync(new CompletedReads()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation("Manga removed from completed reads - تمت إزالة المانجا من المكتمل | MangaId: {Id}, UserId: {UserId}", mangaId, userId);
                return "RemovedFromCompletedReads";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing manga from completed reads - خطأ أثناء الحذف من المكتمل | MangaId: {Id}", mangaId);
                return "ThereWasAProblemDeletingFromCompletedReads";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedCompletedReadsMangaResponse>?)> GetPaginatedCompletedReadsMangaAsync(int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching paginated completed reads list - جلب قائمة المكتمل مع صفحات | Page: {Page}, Size: {Size}", pageNumber, pageSize);

            var flag = await completedReadsRepository.IsAdmin();

            var (message, mangas) = await completedReadsRepository.GetPaginatedCompletedReadsMangaAsync(pageNumber, pageSize, flag);

            return message switch
            {
                "CompletedReadsServiceforRegisteredUsersOnly" => ("CompletedReadsServiceforRegisteredUsersOnly", null),
                "ThereAreNoMangaInYourCompletedReadsList" => ("ThereAreNoMangaInYourCompletedReadsList", null),
                "TheMangaWasFoundInYourCompletedReadsList" => ("TheMangaWasFoundInYourCompletedReadsList", mangas),
                _ => ("ThereAreNoMangaInYourCompletedReadsList", null)
            };
        }
    }
}