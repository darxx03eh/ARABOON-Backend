using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Replies.Commands.Models
{
    public class AddReplyToCommentCommand : IRequest<ApiResponse>
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }
}
