using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface ICompletedReadsService
    {
        public Task<string> AddToCompletedReadsAsync(int mangaId);
        public Task<string> RemoveFromCompletedReadsAsync(int mangaId);
        public Task<(string, PaginatedResult<GetPaginatedCompletedReadsMangaResponse>?)> GetPaginatedCompletedReadsMangaAsync(int pageNumber, int pageSize);

    }
}
