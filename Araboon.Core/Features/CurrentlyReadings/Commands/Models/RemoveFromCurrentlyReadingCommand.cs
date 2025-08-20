using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CurrentlyReadings.Commands.Models
{
    public class RemoveFromCurrentlyReadingCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public RemoveFromCurrentlyReadingCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
