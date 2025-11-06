using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Chapters.Commands.Models
{
    public class UploadChapterImagesCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public IList<IFormFile> Images { get; set; }
    }
}
