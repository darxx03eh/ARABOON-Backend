using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface IReadingLaterService
    {
        public Task<(String, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<String> AddToReadingLaterAsync(Int32 mangaId);
        public Task<String> RemoveFromReadingLaterAsync(Int32 mangaId);
    }
}
