using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class UploadCoverImageCommand : IRequest<ApiResponse>
    {
        public IFormFile OriginalImage { get; set; }
        public IFormFile CroppedImage { get; set; }
    }
}
