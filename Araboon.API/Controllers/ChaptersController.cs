using Araboon.API.Bases;
using Araboon.Core.Features.ChapterImages.Queries.Models;
using Araboon.Core.Features.Chapters.Commands.Models;
using Araboon.Core.Features.Chapters.Queries.Models;
using Araboon.Data.Helpers;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet(Router.ChaptersRouting.ViewChapterImages)]
        public async Task<IActionResult> ViewChapterImage([FromQuery] GetChapterImagesQuery request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPost(Router.ChaptersRouting.ChapterRead)]
        public async Task<IActionResult> ChapterRead([FromBody] ChapterReadCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost(Router.ChaptersRouting.AddNewChapter)]
        public async Task<IActionResult> AddNewChapter([FromForm] AddNewChapterCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete(Router.ChaptersRouting.DeleteExistingChapter)]
        public async Task<IActionResult> DeleteExistingChapter(int id)
        {
            var result = await mediator.Send(new DeleteExistingChapterCommand(id));
            return Result(result);
        }
    }
}
