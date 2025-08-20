using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Favorites.Queries.Models
{
    public class GetPaginatedFavoritesMangaQuery : IRequest<ApiResponse>
    {
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
    }
}
