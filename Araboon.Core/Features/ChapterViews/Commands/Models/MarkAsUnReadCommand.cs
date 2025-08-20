using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ChapterViews.Commands.Models
{
    public class MarkAsUnReadCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public Int32 ChapterID { get; set; }
        public MarkAsUnReadCommand(Int32 mangaId, Int32 chapterId)
            => (MangaID, ChapterID) = (mangaId, chapterId);
    }
}
