using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Ratings.Queries.Models
{
    public class GetRatingForMangaQuery : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public GetRatingForMangaQuery(int id) => Id = id;
    }
}
