using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Favorites.Commands.Models
{
    public class AddToFavoriteCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public AddToFavoriteCommand(int mangaID)
            => MangaID = mangaID;
    }
}
