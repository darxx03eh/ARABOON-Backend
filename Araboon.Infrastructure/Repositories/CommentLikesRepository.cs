using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;

namespace Araboon.Infrastructure.Repositories
{
    public class CommentLikesRepository : GenericRepository<CommentLikes>, ICommentLikesRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CommentLikesRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
    }
}
