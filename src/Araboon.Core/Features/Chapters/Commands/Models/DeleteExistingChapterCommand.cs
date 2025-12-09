using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Chapters.Commands.Models
{
    public class DeleteExistingChapterCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteExistingChapterCommand(int id) => Id = id;
    }
}
