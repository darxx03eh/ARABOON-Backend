using Araboon.API.Bases;
using Araboon.Core.Features.CompletedReads.Commands.Models;
using Araboon.Core.Features.CompletedReads.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class CompletedReadsController : AppBaseController
    {
        [HttpGet(Router.CompletedReadsRouting.ViewCompletedReadsManga)]
        //[ResponseCache(CacheProfileName = "ClientMangaCache")]
        public async Task<IActionResult> ViewCompletedReadsManga(Int32 pageNumber = 1)
        {
            var result = await mediator.Send(new GetPaginatedCompletedReadsMangaQuery() { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpPost(Router.CompletedReadsRouting.AddToCompletedReads)]
        public async Task<IActionResult> AddToCompletedReads(Int32 id)
        {
            var result = await mediator.Send(new AddToCompletedReadsCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.CompletedReadsRouting.RemoveFromCompletedReads)]
        public async Task<IActionResult> RemoveFromCompletedReads(Int32 id)
        {
            var result = await mediator.Send(new RemoveFromCompletedReadsCommand(id));
            return Result(result);
        }
    }
}
