using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Replies.Commands.Models
{
    public class DeleteLikeFromReplyCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteLikeFromReplyCommand(int id)
            => Id = id;
    }
}
