using Araboon.API.Bases;
using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Features.Users.Queries.Models;
using Araboon.Data.Helpers;
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
        [HttpDelete(Router.UserRouting.DeleteCoverImage)]
        public async Task<IActionResult> DeleteCoverImage()
        {
            var result = await mediator.Send(new DeleteCoverImageCommand());
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangeCroppedData)]
        public async Task<IActionResult> ChangeCroppedData(ChangeCroppedDataCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPatch(Router.UserRouting.ChangeCroppedCoverImage)]
        public async Task<IActionResult> ChangeCroppedCoverImage([FromForm] ChangeCroppedCoverImageCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet(Router.UserRouting.GetUsersForDashboard)]
        public async Task<IActionResult> GetUsersForDashboard(string? search, int pageNumber = 1, int pageSize = 5)
        {
            var result = await mediator.Send(new GetUsersForDashboardQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search
            });
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch(Router.UserRouting.ActivateUserToggle)]
        public async Task<IActionResult> ActivateUserToggle(int id)
        {
            var result = await mediator.Send(new ActivateUserToggleCommand(id));
            return Result(result);
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch(Router.UserRouting.ChangeUserRoleToggle)]
        public async Task<IActionResult> ChangeUserRoleToggle(int id)
        {
            var result = await mediator.Send(new ChangeUserRoleToggleCommand(id));
            return Result(result);
        }
    }
}
