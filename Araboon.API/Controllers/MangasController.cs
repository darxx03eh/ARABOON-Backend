using Araboon.API.Bases;
using Araboon.Core.Features.Mangas.Queries.Models;
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
    }
}
