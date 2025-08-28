using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;

namespace Araboon.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<AraboonUser>, IUserRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
    }
}
