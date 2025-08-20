using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CurrentlyReadings.Commands.Models
{
    public class AddToCurrentlyReadingCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public AddToCurrentlyReadingCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
