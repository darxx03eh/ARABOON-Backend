using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Replies.Commands.Models
{
    public class AddReplyToCommentCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}
