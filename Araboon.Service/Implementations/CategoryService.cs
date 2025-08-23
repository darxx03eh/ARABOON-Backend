using Araboon.Data.Entities;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<(string, IList<Category>?)> GetCategoriesAsync()
        {
            var (message, categories) = await categoryRepository.GetCategoriesAsync();
            return message switch
            {
                "CategoriesNotFound" => ("CategoriesNotFound", null),
                "CategoriesFound" => ("CategoriesFound", categories),
                _ => ("AnErrorOccurredWhileRetrievingTheCategorie", null)
            };
        }
    }
}
