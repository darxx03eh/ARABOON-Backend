using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Queries.Models
{
    public class GetDashboardCategoriesQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? search {  get; set; }
    }
}
