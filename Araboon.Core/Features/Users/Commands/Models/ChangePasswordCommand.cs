using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangePasswordCommand : IRequest<ApiResponse>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
