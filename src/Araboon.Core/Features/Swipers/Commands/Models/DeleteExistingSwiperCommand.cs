using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Swipers.Commands.Models
{
    public class DeleteExistingSwiperCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteExistingSwiperCommand(int id) => Id = id;
    }
}
