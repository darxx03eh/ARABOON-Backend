using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CurrentlyReadings.Commands.Models
{
    public class AddToCurrentlyReadingCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public AddToCurrentlyReadingCommand(int mangaId)
            => MangaID = mangaId;
    }
}
