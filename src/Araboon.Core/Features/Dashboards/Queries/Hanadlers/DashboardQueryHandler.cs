using Araboon.Core.Bases;
using Araboon.Core.Features.Dashboards.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Dashboards.Queries.Hanadlers
{
    public class DashboardQueryHandler : ApiResponseHandler
        , IRequestHandler<DashboardStatisticsQuery, ApiResponse>
    {
        private readonly IDashboardService dashboardService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public DashboardQueryHandler(IDashboardService dashboardService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.dashboardService = dashboardService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(DashboardStatisticsQuery request, CancellationToken cancellationToken)
        {
            var (result, statistics) = await dashboardService.DashboardStatisticsAsync();
            return result switch
            {
                "AnErrorOccurredWhileRetrievingDashboardStatistics" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRetrievingDashboardStatistics]),
                "DashboardStatisticsRetrievedSuccessfully" => Success(statistics, message: stringLocalizer[SharedTranslationKeys.DashboardStatisticsRetrievedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRetrievingDashboardStatistics])
            };
        }
    }
}
