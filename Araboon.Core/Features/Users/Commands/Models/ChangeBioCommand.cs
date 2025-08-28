using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeBioCommand : IRequest<ApiResponse>
    {
        public string Bio { get; set; }
    }
}
