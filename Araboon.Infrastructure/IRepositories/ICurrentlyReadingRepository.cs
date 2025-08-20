using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICurrentlyReadingRepository : IGenericRepository<CurrentlyReading>
    {
        public Task<(String, PaginatedResult<GetPaginatedCurrentlyReadingsMangaResponse>?)> GetPaginatedCurrentlyReadingsMangaAsync(Int32 pageNumber, Int32 pageSize);

        public Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId);
    }
}
