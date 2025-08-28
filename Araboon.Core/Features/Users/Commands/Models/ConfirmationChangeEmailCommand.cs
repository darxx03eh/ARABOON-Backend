using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ConfirmationChangeEmailCommand : IRequest<ApiResponse>
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
