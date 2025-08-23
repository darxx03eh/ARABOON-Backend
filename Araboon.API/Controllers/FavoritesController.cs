using Araboon.API.Bases;
using Araboon.Core.Features.Favorites.Commands.Models;
using Araboon.Core.Features.Favorites.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class FavoritesController : AppBaseController
    {
        [HttpGet(Router.FavoritesRouting.ViewFavoritesManga)]
        //[ResponseCache(CacheProfileName = "ClientMangaCache")]
        public async Task<IActionResult> ViewFavoritesManga(int pageNumber = 1)
        {
            var result = await mediator.Send(new GetPaginatedFavoritesMangaQuery() { PageNumber = pageNumber });
            return Result(result);
        }
        [HttpPost(Router.FavoritesRouting.AddToFavorite)]
        public async Task<IActionResult> AddToFavorite(int id)
        {
            var result = await mediator.Send(new AddToFavoriteCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.FavoritesRouting.RemoveFromFavorite)]
        public async Task<IActionResult> RemoveFromFavorite(int id)
        {
            var result = await mediator.Send(new RemoveFromFavoriteCommand(id));
            return Result(result);
        }
    }
}
