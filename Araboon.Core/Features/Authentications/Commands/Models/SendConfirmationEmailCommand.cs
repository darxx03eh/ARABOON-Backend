using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class SendConfirmationEmailCommand : IRequest<ApiResponse>
    {
        public String UserName { get; set; }
        public SendConfirmationEmailCommand(String username)
            => UserName = username;
    }
}
