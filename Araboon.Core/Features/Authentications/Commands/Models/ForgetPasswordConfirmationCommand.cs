using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class ForgetPasswordConfirmationCommand : IRequest<ApiResponse>
    {
        public String Email { get; set; }
        public String Code { get; set; }
    }
}
