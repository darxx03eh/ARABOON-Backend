using Araboon.Data.Entities;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<string> ActivateCategoryAsync(int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound";

            if (category.IsActive)
                return "CategoryAlreadyActive";
            try
            {
                category.IsActive = true;
                category.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.CategoryRepository.UpdateAsync(category);
                return "CategoryActivateSuccessfully";
            }catch(Exception ex)
            {
                return "AnErrorOccurredWhileActivateTheCategory";
            }
        }

        public async Task<(string, int?)> AddNewCategoryAsync(string categoryNameEn, string categoryNameAr)
        {
            var result = await unitOfWork.CategoryRepository.AddAsync(new Category()
            {
                CategoryNameEn = categoryNameEn,
                CategoryNameAr = categoryNameAr,
            });
            if (result is null)
                return ("AnErrorOccurredWhileAddingtheCategory", null);
            return ("CategoryAddedSuccessfully", result.CategoryID);
        }

        public async Task<string> DeActivateCategoryAsync(int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound";

            if (!category.IsActive)
                return "CategoryAlreadDeActive";

            try
            {
                category.IsActive = false;
                await unitOfWork.CategoryRepository.UpdateAsync(category);
                return "CategoryDeActivateSuccessfully";
            }
            catch(Exception exp)
            {
                return "AnErrorOccurredWhileDeActivateTheCategory";
            }
        }

        public async Task<string> DeleteCategoryAsync(int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound";
            try
            {
                await unitOfWork.CategoryRepository.DeleteAsync(category);
                return "CategoryDeletedSuccessfully";
            }
            catch(Exception exp)
            {
                return "AnErrorOccurredWhileDeletingtheCategory";
            }
        }

        public async Task<(string, IList<Category>?)> GetCategoriesAsync()
        {
            var (message, categories) = await unitOfWork.CategoryRepository.GetCategoriesAsync();
            return message switch
            {
                "CategoriesNotFound" => ("CategoriesNotFound", null),
                "CategoriesFound" => ("CategoriesFound", categories),
                _ => ("AnErrorOccurredWhileRetrievingTheCategorie", null)
            };
        }

        public async Task<(string, GetDashboardCategoriesResponse?)> GetCategoryByIdAsync(int id)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return ("CategoryNotFound", null);

            return ("CategoryFound", new GetDashboardCategoriesResponse()
            {
                Id = category.CategoryID,
                En = category.CategoryNameEn,
                Ar = category.CategoryNameAr,
                IsActive = category.IsActive,
                AvailableMangaCounts = category.CategoryMangas.Where(x => x.CategoryID.Equals(category.CategoryID)).Count()
            });
        }

        public async Task<(string, IList<GetDashboardCategoriesResponse>?, CategoryMetaDataRsponse?)> GetDashboardCategoriesAsync(string? search)
        {
            var categoriesQueryable = unitOfWork.CategoryRepository.GetTableNoTracking();
            var meta = new CategoryMetaDataRsponse()
            {
                TotalCategories = await categoriesQueryable.CountAsync(),
                ActiveCategories = await categoriesQueryable.Where(category => category.IsActive).CountAsync(),
                InActiveCategories = await categoriesQueryable.Where(category => !category.IsActive).CountAsync()
            };
            if (!string.IsNullOrWhiteSpace(search))
                categoriesQueryable = categoriesQueryable.Where(
                    category => category.CategoryNameEn.ToLower().Contains(search.ToLower()) ||
                    category.CategoryNameAr.ToLower().Contains(search.ToLower())
                );

            if (categoriesQueryable is null)
                return ("CategoriesNotFound", null, null);

            var categories = await categoriesQueryable.Select(category => new GetDashboardCategoriesResponse()
            {
                Id = category.CategoryID,
                En = category.CategoryNameEn,
                Ar = category.CategoryNameAr,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt.ToString("yyyy-MM-dd"),
                AvailableMangaCounts = category.CategoryMangas.Where(x => x.CategoryID.Equals(category.CategoryID)).Count()
            }).ToListAsync();

            if (categories is null)
                return ("CategoriesNotFound", null, null);

            return ("CategoriesFound", categories, meta);
        }

        public async Task<string> UpdateCategoryAsync(int id, string categoryNameEn, string categoryNameAr)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound";

            try
            {
                category.CategoryNameEn = categoryNameEn;
                category.CategoryNameAr = categoryNameAr;
                await unitOfWork.CategoryRepository.UpdateAsync(category);
                return "CategoryUpdatedSuccessfully";
            }catch(Exception exp)
            {
                return "AnErrorOccurredWhileUpdatingTheCategory";
            }
        }
    }
}
