using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class RatingsRepository : GenericRepository<Ratings>, IRatingsRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RatingsRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Ratings> GetRateByMangaIdAndUserIdAsync(int userId, int mangaId)
            => await GetTableAsTracking().Where(rate => rate.MangaID.Equals(mangaId) && rate.UserID.Equals(userId)).FirstOrDefaultAsync();

        public bool IsUserMakeRateForMangaAsync(int userId, int mangaId)
            => GetTableNoTracking().Any(rate => rate.MangaID.Equals(mangaId) && rate.UserID.Equals(userId));
    }
}
