using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class RegistrationUserCommand : IRequest<ApiResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
