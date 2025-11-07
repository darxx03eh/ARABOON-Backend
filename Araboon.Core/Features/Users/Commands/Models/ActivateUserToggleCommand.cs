using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ActivateUserToggleCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public ActivateUserToggleCommand(int id) => Id = id;
    }
}
