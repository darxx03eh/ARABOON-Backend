using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<(string, IList<Category>?)> GetCategoriesAsync();
        public Task<bool> IsCategoryNameEnExist(string en);
        public Task<bool> IsCategoryNameArExist(string ar);
    }
}
