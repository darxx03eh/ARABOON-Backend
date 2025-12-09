using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ReadingLaters.Queries.Models
{
    public class GetPaginatedReadingLaterMangaQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
