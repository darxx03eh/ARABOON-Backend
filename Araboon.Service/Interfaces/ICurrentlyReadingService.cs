using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface ICurrentlyReadingService
    {
        public Task<(String, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<String> AddToCurrentlyReadingAsync(Int32 mangaId);
        public Task<String> RemoveFromCurrentlyReadingAsync(Int32 mangaId);
    }
}
