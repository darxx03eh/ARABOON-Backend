using Araboon.Data.Response.Comments.Queries;

namespace Araboon.Service.Interfaces
{
    public interface IReplyService
    {
        public Task<(string, GetCommentRepliesResponse?)> AddReplyAsync(string content, int commentId);
        public Task<string> DeleteReplyAsync(int id);
        public Task<(string, string?, string?)> UpdateReplyAsync(string content, int id);
    }
}
