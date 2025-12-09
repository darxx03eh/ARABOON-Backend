using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Chapters.Commands.Models
{
    public class UpdateExistingChapterCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public int ChapterNo { get; set; }
        public string ArabicChapterTitle { get; set; }
        public string EnglishChapterTitle { get; set; }
        public string Language { get; set; }
    }
}
