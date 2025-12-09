using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeUserNameCommand : IRequest<ApiResponse>
    {
        public string UserName { get; set; }
        public ChangeUserNameCommand(string username)
            => UserName = username;
    }
}
