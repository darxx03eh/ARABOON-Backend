using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class RatingsRepository : GenericRepository<Ratings>, IRatingsRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public RatingsRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<Ratings> GetRateByMangaIdAndUserIdAsync(int userId, int mangaId)
            => await GetTableAsTracking().Where(rate => rate.MangaID.Equals(mangaId) && rate.UserID.Equals(userId)).FirstOrDefaultAsync();

        public bool IsUserMakeRateForMangaAsync(int userId, int mangaId)
            => GetTableNoTracking().Any(rate => rate.MangaID.Equals(mangaId) && rate.UserID.Equals(userId));
    }
}
