using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CompletedReads.Commands.Models
{
    public class AddToCompletedReadsCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public AddToCompletedReadsCommand(int mangaId)
            => MangaID = mangaId;
    }
}
