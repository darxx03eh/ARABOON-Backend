using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ReadingLaters.Commands.Models
{
    public class AddToReadingLaterCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public AddToReadingLaterCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
