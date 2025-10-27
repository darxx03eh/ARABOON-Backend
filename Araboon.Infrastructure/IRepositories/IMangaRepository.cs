using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IMangaRepository : IGenericRepository<Manga>
    {
        public Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync(bool isAdmin);
        public Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize, bool isAdmin);
        public Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status,  MangaOrderingEnum orderBy, string? filter, bool isAdmin);
        public Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize, bool isAdmin);
        public Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync(bool isAdmin);
        public Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize, bool isAdmin);
        public Task<(string, PaginatedResult<GetMangaForDashboardResponse>?)> GetMangaForDashboardAsync(string? search, int pageNumber, int pageSize, bool isAdmin);
        public Task<(string, PaginatedResult<GetMangaCommentsResponse>?)> GetMangaCommentsAsync(int id, int pageNumber, int pageSize);
        public Task<int> CommentsCountByIdAsync(int id);
        public Task<bool> IsMangaNameEnExist(string en);
        public Task<bool> IsMangaNameArExist(string ar);
    }
}
