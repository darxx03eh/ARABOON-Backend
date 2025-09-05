using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Comments.Commands.Models
{
    public class AddCommentCommand : IRequest<ApiResponse>
    {
        public int MangaId { get; set; }
        public string Content { get; set; }
    }
}
