using Araboon.Data.Entities;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        public Task<(string, PaginatedResult<GetCommentRepliesResponse>?)> GetCommentRepliesAsync(int id, int pageNumber, int pageSize);
        public Task<(string, int?)> GetCommentsCountForMangaAsync(int id);
    }
}
