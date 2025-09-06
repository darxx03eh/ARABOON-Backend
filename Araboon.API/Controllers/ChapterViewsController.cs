using Araboon.API.Bases;
using Araboon.Core.Features.ChapterViews.Commands.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class ChapterViewsController : AppBaseController
    {
        [HttpPost(Router.ChapterViewRouting.MarkAsRead)]
        public async Task<IActionResult> MarkAsRead(MarkAsReadCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpDelete(Router.ChapterViewRouting.MarkAsUnRead)]
        public async Task<IActionResult> MarkAsUnRead(MarkAsUnReadCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
    }
}
