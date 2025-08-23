using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CompletedReads.Queries.Models
{
    public class GetPaginatedCompletedReadsMangaQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
