using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Notifications.Queries.Models
{
    public class GetPaginatedNotificationsMangaQuery : IRequest<ApiResponse>
    {
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
    }
}
