using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Comments.Commands.Models
{
    public class DeleteLikeFromCommentCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteLikeFromCommentCommand(int id)
            => Id = id;
    }
}
