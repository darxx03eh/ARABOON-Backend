using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Notifications.Commands.Models
{
    public class RemoveFromNotificationsCommand : IRequest<ApiResponse>
    {
        public int MangaID { get; set; }
        public RemoveFromNotificationsCommand(int mangaId)
            => MangaID = mangaId;
    }
}
