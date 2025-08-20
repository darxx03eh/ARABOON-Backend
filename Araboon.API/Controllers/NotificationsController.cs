using Araboon.API.Bases;
using Araboon.Core.Features.Notifications.Commands.Models;
using Araboon.Core.Features.Notifications.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class NotificationsController : AppBaseController
    {
        [HttpGet(Router.NotificationsRouting.ViewNotificationsManga)]
        //[ResponseCache(CacheProfileName = "ClientMangaCache")]
        public async Task<IActionResult> ViewNotificationsManga(Int32 pageNumber = 1)
        {
            var result = await mediator.Send(new GetPaginatedNotificationsMangaQuery() { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpPost(Router.NotificationsRouting.AddToNotifications)]
        public async Task<IActionResult> AddToNotifications(Int32 id)
        {
            var result = await mediator.Send(new AddToNotificationsCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.NotificationsRouting.RemoveFromNotifications)]
        public async Task<IActionResult> RemoveFromNotifications(Int32 id)
        {
            var result = await mediator.Send(new RemoveFromNotificationsCommand(id));
            return Result(result);
        }
    }
}
