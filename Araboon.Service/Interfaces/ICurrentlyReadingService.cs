using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface ICurrentlyReadingService
    {
        public Task<(string, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(int pageNumber, int pageSize);
        public Task<string> AddToCurrentlyReadingAsync(int mangaId);
        public Task<string> RemoveFromCurrentlyReadingAsync(int mangaId);
    }
}
