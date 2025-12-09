using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ReadingLaters.Commands.Models
{
    public class RemoveFromReadingLaterCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public RemoveFromReadingLaterCommand(int mangaId)
            => MangaID = mangaId;
    }
}
