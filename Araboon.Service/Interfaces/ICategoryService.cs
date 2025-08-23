using Araboon.Data.Entities;

namespace Araboon.Service.Interfaces
{
    public interface ICategoryService
    {
        public Task<(string, IList<Category>?)> GetCategoriesAsync();
    }
}
