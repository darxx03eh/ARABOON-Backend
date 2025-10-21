using Araboon.API.Bases;
using Araboon.Core.Features.Categories.Commands.Models;
using Araboon.Core.Features.Categories.Queries.Models;
using Araboon.Data.Helpers;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    public class CategoriesController : AppBaseController
    {
        [AllowAnonymous]
        [HttpGet(Router.CategoryRouting.GetCategories)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await mediator.Send(new GetCategoriesQuery());
            return Result(result);
        }
        [HttpPost(Router.CategoryRouting.AddNewCategory)]
        public async Task<IActionResult> AddNewCategory([FromBody] AddNewCategoryCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpDelete(Router.CategoryRouting.DeleteCategory)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await mediator.Send(new DeleteCategoryCommand(id));
            return Result(result);
        }
        [HttpPost(Router.CategoryRouting.ActivateCategory)]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            var result = await mediator.Send(new ActivateCategoryCommand(id));
            return Result(result);
        }
        [HttpDelete(Router.CategoryRouting.DeActivateCategory)]
        public async Task<IActionResult> DeActivateCategory(int id)
        {
            var result = await mediator.Send(new DeActivateCategoryCommand(id));
            return Result(result);
        }
        [HttpPut(Router.CategoryRouting.UpdateCategory)]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpGet(Router.CategoryRouting.GetDashboardCategories)]
        public async Task<IActionResult> GetDashboardCategories(string? search, int pageNumber = 1, int pageSize = 5)
        {
            var result = await mediator.Send(new GetDashboardCategoriesQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                search = search
            });
            return Result(result);
        }
        [HttpGet(Router.CategoryRouting.GetCategoryById)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await mediator.Send(new GetCategoryByIdQuery(id));
            return Result(result);
        }
    }
}
