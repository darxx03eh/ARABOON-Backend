using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Replies.Commands.Models
{
    public class UpdateReplyCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public UpdateReplyCommand(int id)
            => Id = id;
    }
    public class ReplyDto
    {
        public string Content { get; set; }
    }
}
