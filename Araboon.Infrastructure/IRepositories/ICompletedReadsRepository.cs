using Araboon.Data.Entities;
using Araboon.Data.Response.CompletedReads.Queries;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICompletedReadsRepository : IGenericRepository<CompletedReads>
    {
        public Task<(string, PaginatedResult<GetPaginatedCompletedReadsMangaResponse>?)> GetPaginatedCompletedReadsMangaAsync(int pageNumber, int pageSize);
        public Task<bool> IsMangaExistForUser(int mangaId, int userId);
    }
}
