using Araboon.API.Bases;
using Araboon.Core.Features.Dashboards.Queries.Models;
using Araboon.Data.Helpers;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    public class DashboardsController : AppBaseController
    {
        [HttpGet(Router.DashboardsRouting.DashboardStatistics)]
        public async Task<IActionResult> DashboardStatistics()
        {
            var result = await mediator.Send(new DashboardStatisticsQuery());
            return Result(result);
        }
    }
}
