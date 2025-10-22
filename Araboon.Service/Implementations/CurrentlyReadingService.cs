using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Infrastructure.Repositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class CurrentlyReadingService : ICurrentlyReadingService
    {
        private readonly ICurrentlyReadingRepository currentlyReadingRepository;
        private readonly IMangaRepository mangaRepository;

        public CurrentlyReadingService(ICurrentlyReadingRepository currentlyReadingRepository, IMangaRepository mangaRepository)
        {
            this.currentlyReadingRepository = currentlyReadingRepository;
            this.mangaRepository = mangaRepository;
        }
        public async Task<string> AddToCurrentlyReadingAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = currentlyReadingRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "CurrentlyReadingServiceforRegisteredUsersOnly";
                var exist = await currentlyReadingRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourCurrentlyReadingList";
                await currentlyReadingRepository.AddAsync(new CurrentlyReading()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "AddedToCurrentlyReading";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToCurrentlyReading";
            }
        }
        public async Task<string> RemoveFromCurrentlyReadingAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = currentlyReadingRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "CurrentlyReadingServiceforRegisteredUsersOnly";
                var exist = await currentlyReadingRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourCurrentlyReadingList";
                await currentlyReadingRepository.DeleteAsync(new CurrentlyReading()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "RemovedFromCurrentlyReading";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromCurrentlyReading";
            }
        }
        public async Task<(string, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(int pageNumber, int pageSize)
        {
            var flag = await currentlyReadingRepository.IsAdmin();
            var (message, mangas) = await currentlyReadingRepository.GetPaginatedCurrentlyReadingsMangaAsync(pageNumber, pageSize, flag);
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
