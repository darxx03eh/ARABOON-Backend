using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Notifications.Queries.Models
{
    public class GetPaginatedNotificationsMangaQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
