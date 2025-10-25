using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Queries.Models
{
    public class GetDashboardCategoriesQuery : IRequest<ApiResponse>
    {
        public string? search {  get; set; }
    }
}
