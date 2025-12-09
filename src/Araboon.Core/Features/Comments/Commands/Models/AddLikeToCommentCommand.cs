using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Comments.Commands.Models
{
    public class AddLikeToCommentCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public AddLikeToCommentCommand(int id)
            => Id = id;
    }
}
