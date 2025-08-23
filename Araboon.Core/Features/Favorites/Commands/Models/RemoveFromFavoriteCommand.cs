using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Favorites.Commands.Models
{
    public class RemoveFromFavoriteCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public RemoveFromFavoriteCommand(int mangaID)
            => MangaID = mangaID;
    }
}
