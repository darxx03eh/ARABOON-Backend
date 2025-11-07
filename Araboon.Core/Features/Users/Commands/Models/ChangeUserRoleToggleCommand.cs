using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeUserRoleToggleCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public ChangeUserRoleToggleCommand(int id) => Id = id;
    }
}
