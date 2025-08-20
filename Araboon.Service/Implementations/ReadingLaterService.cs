using Araboon.Data.Entities;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Infrastructure.Repositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class ReadingLaterService : IReadingLaterService
    {
        private readonly IReadingLaterRepository readingLaterRepository;
        private readonly IMangaRepository mangaRepository;

        public ReadingLaterService(IReadingLaterRepository readingLaterRepository, IMangaRepository mangaRepository)
        {
            this.readingLaterRepository = readingLaterRepository;
            this.mangaRepository = mangaRepository;
        }

        public async Task<String> AddToReadingLaterAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = readingLaterRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "ReadingLaterServiceforRegisteredUsersOnly";
                var exist = await readingLaterRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourReadingLaterList";
                await readingLaterRepository.AddAsync(new ReadingLater()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "AddedToReadingLater";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToReadingLater";
            }
        }

        public async Task<String> RemoveFromReadingLaterAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = readingLaterRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "ReadingLaterServiceforRegisteredUsersOnly";
                var exist = await readingLaterRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourReadingLaterList";
                await readingLaterRepository.DeleteAsync(new ReadingLater()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "RemovedFromReadingLater";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromReadingLater";
            }
        }
        public async Task<(String, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            var (message, mangas) = await readingLaterRepository.GetPaginatedReadingLaterMangaAsync(pageNumber, pageSize);
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
