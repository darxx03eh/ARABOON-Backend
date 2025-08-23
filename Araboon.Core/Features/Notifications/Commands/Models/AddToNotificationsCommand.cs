using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Notifications.Commands.Models
{
    public class AddToNotificationsCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public AddToNotificationsCommand(int mangaId)
            => MangaID = mangaId;
    }
}
