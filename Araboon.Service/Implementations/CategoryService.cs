using Araboon.Data.Entities;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<string> ActivateCategoryAsync(int id)
        {
            logger.LogInformation("Activating category - محاولة تفعيل التصنيف - CategoryId: {Id}", id);

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                logger.LogWarning("Category not found - التصنيف غير موجود - CategoryId: {Id}", id);
                return "CategoryNotFound - التصنيف غير موجود";
            }

            if (category.IsActive)
            {
                logger.LogWarning("Category already active - التصنيف مفعل مسبقًا");
                return "CategoryAlreadyActive - التصنيف مفعل مسبقًا";
            }

            try
            {
                var mangasInCategory = await unitOfWork.CategoryMangaRepository.GetTableNoTracking()
                    .CountAsync(x => x.CategoryID == category.CategoryID);

                if (mangasInCategory == 0)
                {
                    logger.LogWarning("No manga attached - لا يمكن تفعيل التصنيف لأنه لا يحتوي مانجات");
                    return "YouCannotActivateTheCategoryBecauseThereAreNoMangaAssociatedWithIt - لا يمكن تفعيل التصنيف لأنه لا يحتوي مانجات";
                }

                category.IsActive = true;
                category.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.CategoryRepository.UpdateAsync(category);

                return "CategoryActivateSuccessfully - تم تفعيل التصنيف بنجاح";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error activating category - خطأ أثناء تفعيل التصنيف");
                return "AnErrorOccurredWhileActivateTheCategory - حدث خطأ أثناء تفعيل التصنيف";
            }
        }

        public async Task<(string, int?)> AddNewCategoryAsync(string categoryNameEn, string categoryNameAr)
        {
            logger.LogInformation("Adding new category - إضافة تصنيف جديد");

            var result = await unitOfWork.CategoryRepository.AddAsync(new Category
            {
                CategoryNameEn = categoryNameEn,
                CategoryNameAr = categoryNameAr
            });

            if (result is null)
            {
                logger.LogError("Failed to add category - فشل في إضافة التصنيف");
                return ("AnErrorOccurredWhileAddingtheCategory - حدث خطأ أثناء إضافة التصنيف", null);
            }

            return ("CategoryAddedSuccessfully - تم إضافة التصنيف بنجاح", result.CategoryID);
        }

        public async Task<string> DeActivateCategoryAsync(int id)
        {
            logger.LogInformation("Deactivating category - تعطيل التصنيف");

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound - التصنيف غير موجود";

            if (!category.IsActive)
                return "CategoryAlreadDeActive - التصنيف معطل مسبقًا";

            try
            {
                category.IsActive = false;
                await unitOfWork.CategoryRepository.UpdateAsync(category);

                return "CategoryDeActivateSuccessfully - تم تعطيل التصنيف بنجاح";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deactivating category - خطأ أثناء التعطيل");
                return "AnErrorOccurredWhileDeActivateTheCategory - حدث خطأ أثناء تعطيل التصنيف";
            }
        }

        public async Task<string> DeleteCategoryAsync(int id)
        {
            logger.LogInformation("Deleting category - حذف التصنيف");

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound - التصنيف غير موجود";

            try
            {
                await unitOfWork.CategoryRepository.DeleteAsync(category);
                return "CategoryDeletedSuccessfully - تم حذف التصنيف بنجاح";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting category - خطأ أثناء حذف التصنيف");
                return "AnErrorOccurredWhileDeletingtheCategory - حدث خطأ أثناء حذف التصنيف";
            }
        }

        public async Task<(string, IList<Category>?)> GetCategoriesAsync()
        {
            logger.LogInformation("Retrieving categories - جلب التصنيفات");

            var (message, categories) = await unitOfWork.CategoryRepository.GetCategoriesAsync();

            return message switch
            {
                "CategoriesNotFound" =>
                    ("CategoriesNotFound - لم يتم العثور على تصنيفات", null),

                "CategoriesFound" =>
                    ("CategoriesFound - تم العثور على التصنيفات", categories),

                _ =>
                    ("AnErrorOccurred - حدث خطأ أثناء جلب التصنيفات", null)
            };
        }

        public async Task<(string, GetDashboardCategoriesResponse?)> GetCategoryByIdAsync(int id)
        {
            logger.LogInformation("Retrieving category by id - جلب التصنيف عبر الرقم");

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return ("CategoryNotFound - التصنيف غير موجود", null);

            var response = new GetDashboardCategoriesResponse
            {
                Id = category.CategoryID,
                En = category.CategoryNameEn,
                Ar = category.CategoryNameAr,
                IsActive = category.IsActive,
                AvailableMangaCounts = category.CategoryMangas.Count()
            };

            return ("CategoryFound - تم العثور على التصنيف", response);
        }

        public async Task<(string, IList<GetDashboardCategoriesResponse>?, CategoryMetaDataRsponse?)> GetDashboardCategoriesAsync(string? search)
        {
            logger.LogInformation("Retrieving dashboard categories - جلب تصنيفات لوحة التحكم");

            var query = unitOfWork.CategoryRepository.GetTableNoTracking();

            var meta = new CategoryMetaDataRsponse
            {
                TotalCategories = await query.CountAsync(),
                ActiveCategories = await query.CountAsync(c => c.IsActive),
                InActiveCategories = await query.CountAsync(c => !c.IsActive)
            };

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.CategoryNameEn.Contains(search) ||
                    x.CategoryNameAr.Contains(search));

            var list = await query.Select(x => new GetDashboardCategoriesResponse
            {
                Id = x.CategoryID,
                En = x.CategoryNameEn,
                Ar = x.CategoryNameAr,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd"),
                AvailableMangaCounts = x.CategoryMangas.Count()
            }).ToListAsync();

            if (!list.Any())
                return ("CategoriesNotFound - لم يتم العثور على تصنيفات", null, null);

            return ("CategoriesFound - تم العثور على التصنيفات", list, meta);
        }

        public async Task<string> UpdateCategoryAsync(int id, string categoryNameEn, string categoryNameAr)
        {
            logger.LogInformation("Updating category - تعديل التصنيف");

            var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return "CategoryNotFound - التصنيف غير موجود";

            try
            {
                category.CategoryNameEn = categoryNameEn;
                category.CategoryNameAr = categoryNameAr;

                await unitOfWork.CategoryRepository.UpdateAsync(category);

                return "CategoryUpdatedSuccessfully - تم تحديث التصنيف بنجاح";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating category - خطأ أثناء التعديل");
                return "AnErrorOccurredWhileUpdatingTheCategory - حدث خطأ أثناء تحديث التصنيف";
            }
        }
    }
}