using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Replies.Commands.Models
{
    public class DeleteReplyCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteReplyCommand(int id)
            => Id = id;
    }
}
