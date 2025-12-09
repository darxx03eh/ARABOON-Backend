using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Favorites.Queries.Models
{
    public class GetPaginatedFavoritesMangaQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
