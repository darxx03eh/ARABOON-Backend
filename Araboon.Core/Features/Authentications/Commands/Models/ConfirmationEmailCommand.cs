using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class ConfirmationEmailCommand : IRequest<ApiResponse>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
