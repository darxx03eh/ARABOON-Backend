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

        public async Task<string> AddToReadingLaterAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = readingLaterRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "ReadingLaterServiceforRegisteredUsersOnly";
                var exist = await readingLaterRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourReadingLaterList";
                await readingLaterRepository.AddAsync(new ReadingLater()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "AddedToReadingLater";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemAddingToReadingLater";
            }
        }

        public async Task<string> RemoveFromReadingLaterAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = readingLaterRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "ReadingLaterServiceforRegisteredUsersOnly";
                var exist = await readingLaterRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourReadingLaterList";
                await readingLaterRepository.DeleteAsync(new ReadingLater()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "RemovedFromReadingLater";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromReadingLater";
            }
        }
        public async Task<(string, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(int pageNumber, int pageSize)
        {
            bool flag = await readingLaterRepository.IsAdmin();
            var (message, mangas) = await readingLaterRepository.GetPaginatedReadingLaterMangaAsync(pageNumber, pageSize, flag);
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
