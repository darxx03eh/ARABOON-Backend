using Araboon.API.Bases;
using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Features.Swipers.Queries.Models;
using Araboon.Data.Helpers;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    public class SwipersController : AppBaseController
    {
        [AllowAnonymous]
        [HttpGet(Router.SwipersRouting.GetSwipersForHomePage)]
        public async Task<IActionResult> GetSwipersForHomePage()
        {
            var result = await mediator.Send(new GetSwipersForHomePageQuery());
            return Result(result);
        }
        [HttpPatch(Router.SwipersRouting.ActivateSwiperToggle)]
        public async Task<IActionResult> ActivateSwiperToggle(int id)
        {
            var result = await mediator.Send(new ActivateSwiperToggleCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.SwipersRouting.DeleteExistingSwiper)]
        public async Task<IActionResult> DeleteExistingSwiper(int id)
        {
            var result = await mediator.Send(new DeleteExistingSwiperCommand(id));
            return Result(result);
        }
        [HttpGet(Router.SwipersRouting.GetSwipersForDashboard)]
        public async Task<IActionResult> GetSwipersForDashboard()
        {
            var result = await mediator.Send(new GetSwipersForDashboardQuery());
            return Result(result);
        }
        [HttpPost(Router.SwipersRouting.AddNewSwiper)]
        public async Task<IActionResult> AddNewSwiper([FromForm] AddNewSwiperCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.SwipersRouting.UploadNewSwiperImage)]
        public async Task<IActionResult> UploadNewSwiperImage([FromForm] UploadNewSwiperImageCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.SwipersRouting.UpdateSwiperNote)]
        public async Task<IActionResult> UpdateSwiperNote([FromBody] UpdateSwiperNoteCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
    }
}
