using Araboon.API.Bases;
using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Features.Authentications.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araboon.API.Controllers
{
    [ApiController]
    public class AuthenticationsController : AppBaseController
    {
        [HttpPost(Router.AuthenticationRouting.RegistrationUser)]
        public async Task<IActionResult> RegistrationUser([FromBody] RegistrationUserCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [EnableRateLimiting("Login")]
        [HttpPost(Router.AuthenticationRouting.SignIn)]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpGet(Router.AuthenticationRouting.EmailConfirmation)]
        public async Task<IActionResult> EmailConfirmation([FromQuery] ConfirmationEmailCommand request)
        {
            var result = await mediator.Send(request);
            if (result.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                return Redirect("https://araboon.vercel.app");
            return Result(result);
        }
        [EnableRateLimiting("SendConfirmationEmail")]
        [HttpPost(Router.AuthenticationRouting.SendConfirmationEmail)]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] SendConfirmationEmailCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [EnableRateLimiting("SendForgetPasswordEmail")]
        [HttpPost(Router.AuthenticationRouting.SendForgetPasswordEmail)]
        public async Task<IActionResult> SendForgetPasswordEmail([FromBody] SendForgetPasswordCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPost(Router.AuthenticationRouting.ForgetPasswordConfirmation)]
        public async Task<IActionResult> ForgetPasswordConfirmation([FromBody] ForgetPasswordConfirmationCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpPost(Router.AuthenticationRouting.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpGet(Router.AuthenticationRouting.ValidateAccessToken)]
        public async Task<IActionResult> ValidateAccessToken([FromQuery] string token)
        {
            var result = await mediator.Send(new ValidateAccessTokenQuery(token));
            return Result(result);
        }
        [HttpPost(Router.AuthenticationRouting.GenerateRefreshToken)]
        public async Task<IActionResult> GenerateRefreshToken()
        {
            var result = await mediator.Send(new GenerateRefreshTokenCommand());
            return Result(result);
        }
        [Authorize]
        [HttpPost(Router.AuthenticationRouting.LogOut)]
        public async Task<IActionResult> LogOut()
        {
            var result = await mediator.Send(new LogOutCommand());
            return Result(result);
        }
    }
}
