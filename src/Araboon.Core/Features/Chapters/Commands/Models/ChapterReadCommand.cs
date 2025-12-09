using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Chapters.Commands.Models
{
    public class ChapterReadCommand : IRequest<ApiResponse>
    {
        public int ChapterId { get; set; }
    }
}
