using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface INotificationsService
    {
        public Task<(string, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(int pageNumber, int pageSize);
        public Task<string> AddToNotificationsAsync(int mangaId);
        public Task<string> RemoveFromNotificationsAsync(int mangaId);
    }
}
