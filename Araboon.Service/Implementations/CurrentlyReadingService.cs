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
        public async Task<String> AddToCurrentlyReadingAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = currentlyReadingRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "CurrentlyReadingServiceforRegisteredUsersOnly";
                var exist = await currentlyReadingRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourCurrentlyReadingList";
                await currentlyReadingRepository.AddAsync(new CurrentlyReading()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "AddedToCurrentlyReading";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToCurrentlyReading";
            }
        }
        public async Task<String> RemoveFromCurrentlyReadingAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = currentlyReadingRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "CurrentlyReadingServiceforRegisteredUsersOnly";
                var exist = await currentlyReadingRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourCurrentlyReadingList";
                await currentlyReadingRepository.DeleteAsync(new CurrentlyReading()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "RemovedFromCurrentlyReading";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromCurrentlyReading";
            }
        }
        public async Task<(String, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            var (message, mangas) = await currentlyReadingRepository.GetPaginatedCurrentlyReadingsMangaAsync(pageNumber, pageSize);
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
