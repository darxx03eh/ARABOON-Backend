using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class ConfirmationEmailCommand : IRequest<ApiResponse>
    {
        public String Email { get; set; }
        public String Token { get; set; }
    }
}
