using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class ResetPasswordCommand : IRequest<ApiResponse>
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
