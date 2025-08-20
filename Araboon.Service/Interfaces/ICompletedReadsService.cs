using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface ICompletedReadsService
    {
        public Task<String> AddToCompletedReadsAsync(Int32 mangaId);
        public Task<String> RemoveFromCompletedReadsAsync(Int32 mangaId);
        public Task<(String, PaginatedResult<GetPaginatedCompletedReadsMangaResponse>?)> GetPaginatedCompletedReadsMangaAsync(Int32 pageNumber, Int32 pageSize);

    }
}
