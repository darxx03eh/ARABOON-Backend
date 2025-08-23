using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IReadingLaterRepository : IGenericRepository<ReadingLater>
    {
        public Task<(string, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(int pageNumber, int pageSize);
        public Task<bool> IsMangaExistForUser(int mangaId, int userId);
    }
}
