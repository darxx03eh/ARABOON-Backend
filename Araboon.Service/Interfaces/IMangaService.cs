using Araboon.Data.DTOs.Mangas;
using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Araboon.Service.Interfaces
{
    public interface IMangaService
    {
        public Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync();
        public Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync();
        public Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize);
        public Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int PageNumber, int pageSize);
        public Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter);
        public Task<(string, Manga?, string?, int?)> GetMangaByIDAsync(int id);
        public Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize);
        public Task<(string, PaginatedResult<GetMangaForDashboardResponse>?)> GetMangaForDashboardAsync(string? search, int pageNumber, int pageSize);
        public Task<(string, PaginatedResult<GetMangaCommentsResponse>?)> GetMangaCommentsAsync(int id, int pageNumber, int pageSize);
        public Task<(string, int?)> GetCommentsCountAsync(int id);
        public Task<(string, GetMangaForDashboardResponse?)> AddNewMangaAsync(MangaInfoDTO mangaInfo);
        public Task<(string, GetMangaForDashboardResponse?)> UpdateExistMangaAsync(UpdateMangaInfoDTO mangaInfo, int mangaId);
        public Task<string> DeleteMangaAsync(int id);
        public Task<string> DeleteMangaImageAsync(int id);
        public Task<(string, string?)> UploadMangaImageAsync(int id, IFormFile image);
        public Task<string> MakeArabicAvailableOrUnAvailableAsync(int id);
        public Task<string> MakeEnglishAvailableOrUnAvailableAsync(int id);
        public Task<string> ActivateAndDeActivateMangaAsync(int id);
    }
}
