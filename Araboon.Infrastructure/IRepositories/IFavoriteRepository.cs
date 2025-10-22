using Araboon.Data.Entities;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        public Task<(string, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(int pageNumber, int pageSize, bool isAdmin);
        public Task<bool> IsMangaExistForUser(int mangaId, int userId);
    }
}
