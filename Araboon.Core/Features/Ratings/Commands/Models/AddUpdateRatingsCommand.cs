using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Ratings.Commands.Models
{
    public class AddUpdateRatingsCommand : IRequest<ApiResponse>
    {
        public int MangaId { get; set; }
        public double Rate { get; set; }
    }
}
