using Araboon.Data.Entities;
using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICompletedReadsRepository : IGenericRepository<CompletedReads>
    {
        public Task<(String, PaginatedResult<GetPaginatedCompletedReadsMangaResponse>?)> GetPaginatedCompletedReadsMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId);
    }
}
