using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CompletedReads.Commands.Models
{
    public class RemoveFromCompletedReadsCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public RemoveFromCompletedReadsCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
