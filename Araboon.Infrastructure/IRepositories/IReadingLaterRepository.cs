using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IReadingLaterRepository : IGenericRepository<ReadingLater>
    {
        public Task<(String, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId);
    }
}
