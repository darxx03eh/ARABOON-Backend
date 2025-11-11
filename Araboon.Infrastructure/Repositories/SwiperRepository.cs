using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class SwiperRepository : GenericRepository<Swiper>, ISwiperRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public SwiperRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager) 
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<bool> IsLinkExistsAsync(string link, int? excludeSwiperId = null)
        {
            var query = GetTableNoTracking().Where(
                swiper => swiper.Link.ToLower().Equals(link.ToLower())
            );

            if (excludeSwiperId.HasValue)
                query = query.Where(swiper => !swiper.SwiperId.Equals(excludeSwiperId));

            return await query.AnyAsync();
        }
    }
}
