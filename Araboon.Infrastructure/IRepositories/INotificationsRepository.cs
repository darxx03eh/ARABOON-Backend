using Araboon.Data.Entities;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface INotificationsRepository : IGenericRepository<Notifications>
    {
        public Task<(String, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId);
    }
}
