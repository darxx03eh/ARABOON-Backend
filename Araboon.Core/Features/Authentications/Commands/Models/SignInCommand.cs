using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class SignInCommand : IRequest<ApiResponse>
    {
        public String UserName { get; set; }
        public String Password { get; set; }
    }
}
