using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CategoryRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string, IList<Category>?)> GetCategoriesAsync()
        {
            var categories = await GetTableNoTracking().ToListAsync();
            if (categories.Count().Equals(0))
                return ("CategoriesNotFound", null);
            return ("CategoriesFound", categories);
        }
    }
}
