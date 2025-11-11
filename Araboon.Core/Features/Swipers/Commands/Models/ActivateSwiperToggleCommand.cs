using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Swipers.Commands.Models
{
    public class ActivateSwiperToggleCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public ActivateSwiperToggleCommand(int id) => Id = id;
    }
}
