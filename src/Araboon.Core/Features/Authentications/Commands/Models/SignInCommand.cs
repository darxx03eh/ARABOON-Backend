using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class SignInCommand : IRequest<ApiResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
