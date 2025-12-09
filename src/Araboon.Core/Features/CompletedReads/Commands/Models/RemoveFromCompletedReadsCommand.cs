using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CompletedReads.Commands.Models
{
    public class RemoveFromCompletedReadsCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public RemoveFromCompletedReadsCommand(int mangaId)
            => MangaID = mangaId;
    }
}
