using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class ForgetPasswordConfirmationCommand : IRequest<ApiResponse>
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
