using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface INotificationsService
    {
        public Task<(String, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<String> AddToNotificationsAsync(Int32 mangaId);
        public Task<String> RemoveFromNotificationsAsync(Int32 mangaId);
    }
}
