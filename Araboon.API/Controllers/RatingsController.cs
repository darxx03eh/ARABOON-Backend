using Araboon.API.Bases;
using Araboon.Core.Features.Ratings.Commands.Models;
using Araboon.Core.Features.Ratings.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class RatingsController : AppBaseController
    {
        [HttpPut(Router.RatingRouting.prefix)]
        public async Task<IActionResult> Rate([FromBody] AddUpdateRatingsCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpDelete(Router.RatingRouting.DeleteRate)]
        public async Task<IActionResult> DeleteRate(int id)
        {
            var result = await mediator.Send(new DeleteRatingsCommand(id));
            return Result(result);
        }
        [HttpGet(Router.RatingRouting.GetRate)]
        public async Task<IActionResult> GetRate(int id)
        {
            var result = await mediator.Send(new GetRatingForMangaQuery(id));
            return Result(result);
        }
    }
}
