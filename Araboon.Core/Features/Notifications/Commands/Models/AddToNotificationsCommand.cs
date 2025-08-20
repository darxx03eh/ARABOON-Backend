using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Notifications.Commands.Models
{
    public class AddToNotificationsCommand : IRequest<ApiResponse>
    {
        public Int32 MangaID { get; set; }
        public AddToNotificationsCommand(Int32 mangaId)
            => MangaID = mangaId;
    }
}
