using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Comments.Commands.Models
{
    public class UpdateCommentCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public UpdateCommentCommand(int id)
            => Id = id;
    }
    public class CommentDTO
    {
        public string Content { get; set; }
    }
}
