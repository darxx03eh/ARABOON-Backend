using Araboon.Data.Response.Dashboards.Queries;

namespace Araboon.Service.Interfaces
{
    public interface IDashboardService
    {
        public Task<(string, DashboardStatisticsResponse)> DashboardStatisticsAsync();
    }
}
