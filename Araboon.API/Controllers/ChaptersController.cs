using Araboon.API.Bases;
using Araboon.Core.Features.Chapters.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [ApiController]
    public class ChaptersController : AppBaseController
    {
        [HttpGet(Router.ChaptersRouting.ViewChaptersForSpecificMangaByLanguage)]
        public async Task<IActionResult> ViewChaptersForSpecificMangaByLanguage([FromQuery]GetChaptersForSpecificMangaByLanguageQuery request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
    }
}
