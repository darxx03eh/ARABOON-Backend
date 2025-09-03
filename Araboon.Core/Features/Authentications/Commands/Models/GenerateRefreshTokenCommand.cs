using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class GenerateRefreshTokenCommand : IRequest<ApiResponse>
    {
    }
}
