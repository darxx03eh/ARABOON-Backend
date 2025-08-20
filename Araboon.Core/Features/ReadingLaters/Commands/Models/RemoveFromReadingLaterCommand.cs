using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ReadingLaters.Commands.Models
{
    public class RemoveFromReadingLaterCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public RemoveFromReadingLaterCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
