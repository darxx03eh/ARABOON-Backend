using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Favorites.Commands.Models
{
    public class RemoveFromFavoriteCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public RemoveFromFavoriteCommand(Int32 mangaID)
            => MangaID = mangaID;
    }
}
