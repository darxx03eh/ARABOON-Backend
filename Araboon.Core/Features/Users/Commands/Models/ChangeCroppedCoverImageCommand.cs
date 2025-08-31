using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class ChangeCroppedCoverImageCommand : IRequest<ApiResponse>
    {
        public IFormFile Image { get; set; }
    }
}
