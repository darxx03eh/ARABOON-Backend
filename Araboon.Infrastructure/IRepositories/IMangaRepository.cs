using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IMangaRepository : IGenericRepository<Manga>
    {
        public Task<(String, Dictionary<String, IList<GetCategoriesHomePageResponse>>?)> GetCategoriesHomePageAsync();
        public Task<(String, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(String category, Int32 pageNumber, Int32 pageSize);
        public Task<(String, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(Int32 pageNumber, Int32 pageSize, String status,  MangaOrderingEnum orderBy, String? filter);
        public Task<(String, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<(String, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync();
    }
}
