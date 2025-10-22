using Araboon.Data.Entities;
using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Infrastructure.Repositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class CompletedReadsService : ICompletedReadsService
    {
        private readonly ICompletedReadsRepository completedReadsRepository;
        private readonly IMangaRepository mangaRepository;

        public CompletedReadsService(ICompletedReadsRepository completedReadsRepository, IMangaRepository mangaRepository)
        {
            this.completedReadsRepository = completedReadsRepository;
            this.mangaRepository = mangaRepository;
        }
        public async Task<string> AddToCompletedReadsAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = completedReadsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "CompletedReadsServiceforRegisteredUsersOnly";
                var exist = await completedReadsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourCompletedReadsList";
                await completedReadsRepository.AddAsync(new CompletedReads()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "AddedToCompletedReads";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToCompletedReads";
            }
        }
        public async Task<string> RemoveFromCompletedReadsAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = completedReadsRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "CompletedReadsServiceforRegisteredUsersOnly";
                var exist = await completedReadsRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourCompletedReadsList";
                await completedReadsRepository.DeleteAsync(new CompletedReads()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "RemovedFromCompletedReads";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromCompletedReads";
            }
        }
        public async Task<(string, PaginatedResult<GetPaginatedCompletedReadsMangaResponse>?)> GetPaginatedCompletedReadsMangaAsync(int pageNumber, int pageSize)
        {
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
