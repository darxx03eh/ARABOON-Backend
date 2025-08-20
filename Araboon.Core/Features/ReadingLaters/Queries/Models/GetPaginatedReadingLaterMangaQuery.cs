using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.ReadingLaters.Queries.Models
{
    public class GetPaginatedReadingLaterMangaQuery : IRequest<ApiResponse>
    {
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
    }
}
