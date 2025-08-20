using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Favorites.Commands.Models
{
    public class AddToFavoriteCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public AddToFavoriteCommand(Int32 mangaID)
            => MangaID = mangaID;
    }
}
