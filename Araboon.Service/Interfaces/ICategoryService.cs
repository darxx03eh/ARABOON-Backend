using Araboon.Data.Entities;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface ICategoryService
    {
        public Task<(string, IList<Category>?)> GetCategoriesAsync();
        public Task<(string, PaginatedResult<GetDashboardCategoriesResponse>?, CategoryMetaDataRsponse?)> GetDashboardCategoriesAsync(int pageNumber, int pageSize, string? search);
        public Task<(string, GetDashboardCategoriesResponse?)> GetCategoryByIdAsync(int id);
        public Task<(string, int?)> AddNewCategoryAsync(string categoryNameEn, string categoryNameAr);
        public Task<string> UpdateCategoryAsync(int id, string categoryNameEn, string categoryNameAr);
        public Task<string> DeleteCategoryAsync(int id);
        public Task<string> ActivateCategoryAsync(int id);
        public Task<string> DeActivateCategoryAsync(int id);
    }
}
