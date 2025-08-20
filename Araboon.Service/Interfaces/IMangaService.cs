using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface IMangaService
    {
        public Task<(String, Dictionary<String, IList<GetCategoriesHomePageResponse>>?)> GetCategoriesHomePageAsync();
        public Task<(String, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync();
        public Task<(String, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(Int32 pageNumber, Int32 pageSize);
        public Task<(String, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(String category, Int32 PageNumber, Int32 pageSize);
        public Task<(String, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(Int32 pageNumber, Int32 pageSize, String status, MangaOrderingEnum orderBy, String? filter);
        public Task<(String, Manga?)> GetMangaByIDAsync(Int32 id);
    }
}
