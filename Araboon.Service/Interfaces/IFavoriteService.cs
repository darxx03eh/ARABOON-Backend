using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface IFavoriteService
    {
        public Task<String> AddToFavoriteAsync(Int32 mangaId);
        public Task<String> RemoveFromFavoriteAsync(Int32 mangaId);
        public Task<(String, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(Int32 pageNumber, Int32 pageSize);
    }
}
