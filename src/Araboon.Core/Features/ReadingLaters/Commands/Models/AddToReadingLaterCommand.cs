using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ReadingLaters.Commands.Models
{
    public class AddToReadingLaterCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public AddToReadingLaterCommand(int mangaId)
            => MangaID = mangaId;
    }
}
