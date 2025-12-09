using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Swipers.Commands.Models
{
    public class AddNewSwiperCommand : IRequest<ApiResponse>
    {
        public IFormFile Image { get; set; }
        public string? NoteEn { get; set; }
        public string? NoteAr {  get; set; }
        public string Link { get; set; }
    }
}
