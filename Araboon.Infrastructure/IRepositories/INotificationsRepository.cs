using Araboon.Data.Entities;
using Araboon.Data.Response.Notifications.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface INotificationsRepository : IGenericRepository<Notifications>
    {
        public Task<(string, PaginatedResult<GetPaginatedNotificationsMangaResponse>?)> GetPaginatedNotificationsMangaAsync(int pageNumber, int pageSize, bool isAdmin);
        public Task<bool> IsMangaExistForUser(int mangaId, int userId);
        public Task<IList<(string Name, string Email)>> GetEmailsToNewChapterNotify(int mangaId);
    }
}
