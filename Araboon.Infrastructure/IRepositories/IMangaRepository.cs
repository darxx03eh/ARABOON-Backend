using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IMangaRepository : IGenericRepository<Manga>
    {
        public Task<(string, Dictionary<string, IList<GetCategoriesHomePageResponse>>?)> GetCategoriesHomePageAsync();
        public Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize);
        public Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status,  MangaOrderingEnum orderBy, string? filter);
        public Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize);
        public Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync();
    }
}
