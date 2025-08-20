using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CompletedReads.Commands.Models
{
    public class AddToCompletedReadsCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public AddToCompletedReadsCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
