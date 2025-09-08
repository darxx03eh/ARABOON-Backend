using Araboon.Data.Response.Comments.Queries;
using Araboon.Data.Wrappers;

namespace Araboon.Service.Interfaces
{
    public interface ICommentService
    {
        public Task<string> AddCommentAsync(string content, int mangaId);
        public Task<string> DeleteCommentAsync(int id);
        public Task<string> UpdateCommentAsync(string content, int id);
        public Task<string> AddLikeAsync(int id);
        public Task<string> DeleteLikeAsync(int id);
        public Task<(string, PaginatedResult<GetCommentRepliesResponse>?)> GetCommentRepliesAsync(int id, int pageNumber, int pageSize);
    }
}
