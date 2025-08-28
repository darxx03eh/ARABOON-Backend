using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeEmailCommand : IRequest<ApiResponse>
    {
        public string Email { get; set; }
    }
}
