using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface IFavoriteService
    {
        public Task<string> AddToFavoriteAsync(int mangaId);
        public Task<string> RemoveFromFavoriteAsync(int mangaId);
        public Task<(string, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(int pageNumber, int pageSize);
    }
}
