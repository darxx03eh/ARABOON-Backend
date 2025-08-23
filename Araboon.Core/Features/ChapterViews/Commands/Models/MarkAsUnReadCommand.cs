using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ChapterViews.Commands.Models
{
    public class MarkAsUnReadCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public int ChapterID { get; set; }
        public MarkAsUnReadCommand(int mangaId, int chapterId)
            => (MangaID, ChapterID) = (mangaId, chapterId);
    }
}
