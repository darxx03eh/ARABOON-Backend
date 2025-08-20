using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CompletedReads.Queries.Models
{
    public class GetPaginatedCompletedReadsMangaQuery : IRequest<ApiResponse>
    {
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
    }
}
