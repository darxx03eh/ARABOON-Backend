using Araboon.API.Bases;
using Araboon.Core.Features.Categories.Queries.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [ApiController]
    public class CategoriesController : AppBaseController
    {
        [HttpGet(Router.CategoryRouting.GetCategories)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await mediator.Send(new GetCategoriesQuery());
            return Result(result);
        }
    }
}
