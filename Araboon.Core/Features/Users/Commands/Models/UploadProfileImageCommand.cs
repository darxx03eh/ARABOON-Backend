using Araboon.Core.Bases;
using Araboon.Data.Response.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Users.Commands.Models
{
    public class UploadProfileImageCommand : IRequest<ApiResponse>
    {
        public IFormFile ProfileImage { get; set; }
        public CropData CropData { get; set; } 
    }
}
