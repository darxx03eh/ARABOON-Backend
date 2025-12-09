using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Comments.Commands.Models
{
    public class DeleteCommentCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteCommentCommand(int id)
            => Id = id;
    }
}
