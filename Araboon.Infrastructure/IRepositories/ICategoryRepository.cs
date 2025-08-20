using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public Task<(String, IList<Category>?)> GetCategoriesAsync();
    }
}
