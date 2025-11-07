using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Queries.Models
{
    public class GetUsersForDashboardQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search {  get; set; }
    }
}
