using Araboon.API.Bases;
using Araboon.Core.Features.ReadingLaters.Commands.Models;
using Araboon.Core.Features.ReadingLaters.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class ReadingLatersController : AppBaseController
    {
        [HttpGet(Router.ReadingLaterRouting.ViewReadingLaterManga)]
        //[ResponseCache(CacheProfileName = "ClientMangaCache")]
        public async Task<IActionResult> ViewReadingLaterManga(int pageNumber = 1)
        {
            var result = await mediator.Send(new GetPaginatedReadingLaterMangaQuery() { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpPost(Router.ReadingLaterRouting.AddToReadingLater)]
        public async Task<IActionResult> AddToReadingLater(int id)
        {
            var result = await mediator.Send(new AddToReadingLaterCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.ReadingLaterRouting.RemoveFromReadingLater)]
        public async Task<IActionResult> RemoveFromReadingLater(int id)
        {
            var result = await mediator.Send(new RemoveFromReadingLaterCommand(id));
            return Result(result);
        }
    }
}
