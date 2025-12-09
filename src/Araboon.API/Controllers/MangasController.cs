using Araboon.API.Bases;
using Araboon.Core.Features.Mangas.Commands.Models;
using Araboon.Core.Features.Mangas.Queries.Models;
using Araboon.Data.Helpers;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [ApiController]
    public class MangasController : AppBaseController
    {
        [HttpGet(Router.MangaRouting.GetCategoriesHomePageMangas)]
        //[ResponseCache(CacheProfileName = "DefaultCache")]
        public async Task<IActionResult> GetCategoriesHomePageMangas()
        {
            var result = await mediator.Send(new GetCategoriesHomePageQuery());
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.GetHottestMangas)]
        //[ResponseCache(CacheProfileName = "DefaultCache")]
        public async Task<IActionResult> GetHottestMangas()
        {
            var result = await mediator.Send(new GetHottestMangasQuery());
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.GetMangaByID)]
        //[ResponseCache(CacheProfileName = "DefaultCache")]
        public async Task<IActionResult> GetMangaByID(int id)
        {
            var result = await mediator.Send(new GetMangaByIDQuery(id));
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.GetMangaByCategoryName)]
        //[ResponseCache(CacheProfileName = "PageNumberCache")]
        public async Task<IActionResult> GetMangaByCategoryName(string category, int pageNumber = 1)
        {
            var result = await mediator.Send(new GetMangaByCategoryNameQuery(category) { PageNumber = pageNumber});
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.GetPaginatedHottestManga)]
        //[ResponseCache(CacheProfileName = "PageNumberCache")]
        public async Task<IActionResult> GetPaginatedHottestManga(int pageNumber = 1)
        {
            var result = await mediator.Send(new GetPaginatedHottestMangaQuery() { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.GetMangaByStatus)]
        //[ResponseCache(CacheProfileName = "PageNumberCache", VaryByQueryKeys = ["Status", "Filter", "OrderBy", "PageNumber"])]
        public async Task<IActionResult> GetMangaByStatus([FromQuery]GetMangaByStatusQuery request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.MangaSearch)]
        public async Task<IActionResult> MangaSearch([FromQuery] MangaSearchQuery request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }

        [HttpGet(Router.MangaRouting.GetMangaComments)]
        public async Task<IActionResult> GetMangaComments(int id, int pageNumber = 1)
        {
            var result = await mediator.Send(new GetMangaCommentsQuery(id) { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpGet(Router.MangaRouting.GetMangaCommentsCounts)]
        public async Task<IActionResult> GetMangaCommentsCounts(int id)
        {
            var result = await mediator.Send(new GetCommentsCountQuery(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPost(Router.MangaRouting.AddNewManga)]
        public async Task<IActionResult> AddNewManga([FromForm] AddNewMangaCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete(Router.MangaRouting.DeleteManga)]
        public async Task<IActionResult> DeleteManga(int id)
        {
            var result = await mediator.Send(new DeleteMangaCommand(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete(Router.MangaRouting.DeleteMangaImage)]
        public async Task<IActionResult> DeleteMangaImage(int id)
        {
            var result = await mediator.Send(new DeleteMangaImageCommand(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch(Router.MangaRouting.UploadNewImage)]
        public async Task<IActionResult> UploadNewImage([FromForm] UploadNewMangaImageCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch(Router.MangaRouting.ArabicAvailableOrUnAvailable)]
        public async Task<IActionResult> ArabicAvailableOrUnAvailable(int id)
        {
            var result = await mediator.Send(new MakeArabicAvailableOrUnAvailableCommand(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch(Router.MangaRouting.EnglishAvailableOrUnAvailableAsync)]
        public async Task<IActionResult> EnglishAvailableOrUnAvailableAsync(int id)
        {
            var result = await mediator.Send(new MakeEnglishAvailableOrUnAvailableCommand(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch(Router.MangaRouting.ActivateOrDeActivateManga)]
        public async Task<IActionResult> ActivateOrDeActivateManga(int id)
        {
            var result = await mediator.Send(new ActivateOrDeActivateMangaCommand(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut(Router.MangaRouting.UpdateMangaInfo)]
        public async Task<IActionResult> UpdateMangaInfo([FromBody] UpdateMangaCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet(Router.MangaRouting.GetMangaForDashboard)]
        public async Task<IActionResult> GetMangaForDashboard([FromQuery] GetMangaForDashboardQuery request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
    }
}
