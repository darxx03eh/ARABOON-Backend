using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Araboon.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<UserRefreshToken>, IRefreshTokenRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public RefreshTokenRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }
    }
}
