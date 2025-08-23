using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ChapterViews.Commands.Models
{
    public class MarkAsReadCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public int ChapterID { get; set; }
        public MarkAsReadCommand(int mangaId, int chapterId)
            => (MangaID, ChapterID) = (mangaId, chapterId);
    }
}
