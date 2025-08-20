using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Notifications.Commands.Models
{
    public class RemoveFromNotificationsCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public RemoveFromNotificationsCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
