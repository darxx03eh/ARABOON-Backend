using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ChapterViews.Commands.Models
{
    public class MarkAsReadCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public Int32 ChapterID { get; set; }
        public MarkAsReadCommand(Int32 mangaId, Int32 chapterId)
            => (MangaID, ChapterID) = (mangaId, chapterId);
    }
}
