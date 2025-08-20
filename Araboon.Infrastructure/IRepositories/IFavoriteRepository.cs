using Araboon.Data.Entities;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        public Task<(String, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId);
    }
}
