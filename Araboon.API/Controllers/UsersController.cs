using Araboon.API.Bases;
using Araboon.Core.Features.Users.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [ApiController]
    public class UsersController : AppBaseController
    {
        [HttpGet(Router.UserRouting.GetUserProfile)]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            var result = await mediator.Send(new GetUserProfileQuery(username));
            return Result(result);
        }
    }
}
