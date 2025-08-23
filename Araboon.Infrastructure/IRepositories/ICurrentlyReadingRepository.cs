using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICurrentlyReadingRepository : IGenericRepository<CurrentlyReading>
    {
        public Task<(string, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(int pageNumber, int pageSize);

        public Task<bool> IsMangaExistForUser(int mangaId, int userId);
    }
}
