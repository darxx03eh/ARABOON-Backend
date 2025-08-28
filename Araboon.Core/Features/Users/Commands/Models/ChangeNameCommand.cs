using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeNameCommand : IRequest<ApiResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
