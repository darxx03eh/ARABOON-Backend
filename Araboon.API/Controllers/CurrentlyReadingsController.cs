using Araboon.API.Bases;
using Araboon.Core.Features.CurrentlyReadings.Commands.Models;
using Araboon.Core.Features.CurrentlyReadings.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class CurrentlyReadingsController : AppBaseController
    {
        [HttpGet(Router.CurrentlyReadingRouting.ViewCurrentlyReadingManga)]
        //[ResponseCache(CacheProfileName = "ClientMangaCache")]
        public async Task<IActionResult> ViewCurrentlyReadingManga(Int32 pageNumber = 1)
        {
            var result = await mediator.Send(new GetPaginatedCurrentlyReadingMangaQuery() { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpPost(Router.CurrentlyReadingRouting.AddToCurrentlyReading)]
        public async Task<IActionResult> AddToCurrentlyReading(Int32 id)
        {
            var result = await mediator.Send(new AddToCurrentlyReadingCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.CurrentlyReadingRouting.RemoveFromCurrentlyReading)]
        public async Task<IActionResult> RemoveFromCurrentlyReading(Int32 id)
        {
            var result = await mediator.Send(new RemoveFromCurrentlyReadingCommand(id));
            return Result(result);
        }
    }
}
