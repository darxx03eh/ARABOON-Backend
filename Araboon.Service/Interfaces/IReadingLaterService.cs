using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface IReadingLaterService
    {
        public Task<(string, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(int pageNumber, int pageSize);
        public Task<string> AddToReadingLaterAsync(int mangaId);
        public Task<string> RemoveFromReadingLaterAsync(int mangaId);
    }
}
