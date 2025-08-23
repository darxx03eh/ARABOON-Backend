using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CurrentlyReadings.Commands.Models
{
    public class RemoveFromCurrentlyReadingCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public RemoveFromCurrentlyReadingCommand(int mangaId)
            => MangaID = mangaId;
    }
}
