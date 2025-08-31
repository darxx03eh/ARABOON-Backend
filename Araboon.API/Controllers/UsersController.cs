using Araboon.API.Bases;
using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Features.Users.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [ApiController]
    [Authorize]
    public class UsersController : AppBaseController
    {
        [AllowAnonymous]
        [HttpGet(Router.UserRouting.Profile)]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            var result = await mediator.Send(new GetUserProfileQuery(username));
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangePassword)]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangeUserName)]
        public async Task<IActionResult> ChangeUserName(ChangeUserNameCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPut(Router.UserRouting.UploadProfileImage)]
        public async Task<IActionResult> UploadProfileImage([FromForm] UploadProfileImageCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPut(Router.UserRouting.UploadCoverImage)]
        public async Task<IActionResult> UploadCoverImage([FromForm] UploadCoverImageCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangeEmail)]
        public async Task<IActionResult> ChangeEmail(ChangeEmailCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [AllowAnonymous]
        [HttpGet(Router.UserRouting.ChangeEmailConfirmation)]
        public async Task<IActionResult> ChangeEmailConfirmation([FromQuery] ConfirmationChangeEmailCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangeBio)]
        public async Task<IActionResult> ChangeBio(ChangeBioCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangeName)]
        public async Task<IActionResult> ChangeName(ChangeNameCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpDelete(Router.UserRouting.DeleteProfileImage)]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var result = await mediator.Send(new DeleteProfileImageCommand());
            return Result(result);
        }
    }
}
