using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.CurrentlyReadings.Queries.Models
{
    public class GetPaginatedCurrentlyReadingMangaQuery : IRequest<ApiResponse>
    {
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
    }
}
