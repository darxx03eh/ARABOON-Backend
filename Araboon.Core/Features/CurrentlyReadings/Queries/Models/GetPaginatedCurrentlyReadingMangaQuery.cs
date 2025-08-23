using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CurrentlyReadings.Queries.Models
{
    public class GetPaginatedCurrentlyReadingMangaQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
