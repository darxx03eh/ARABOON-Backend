using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Swipers.Commands.Models
{
    public class UpdateSwiperNoteLinkCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string Link { get; set; }
    }
}
