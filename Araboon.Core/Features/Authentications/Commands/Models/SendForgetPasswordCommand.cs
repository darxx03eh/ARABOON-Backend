using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class SendForgetPasswordCommand : IRequest<ApiResponse>
    {
        public String Email { get; set; }
        public SendForgetPasswordCommand(String email)
            => Email = email;
    }
}
