using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Replies.Commands.Models
{
    public class AddLikeToReplyCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public AddLikeToReplyCommand(int id)
            => Id = id;
    }
}
