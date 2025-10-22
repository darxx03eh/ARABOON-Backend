using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public CategoryRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager) 
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
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
