using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Ratings.Commands.Models
{
    public class DeleteRatingsCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteRatingsCommand(int id) => Id = id;
    }
}
