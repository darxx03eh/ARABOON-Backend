using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Bases
{
    [ApiController]
    public class AppBaseController : ControllerBase
    {
        private IMediator instanceMediator;
        protected IMediator mediator => instanceMediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected ObjectResult Result(ApiResponse response) => response.StatusCode switch
        {
            System.Net.HttpStatusCode.OK => new OkObjectResult(response),
            System.Net.HttpStatusCode.Created => new CreatedResult(String.Empty, response),
            System.Net.HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
            System.Net.HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
            System.Net.HttpStatusCode.NotFound => new NotFoundObjectResult(response),
            System.Net.HttpStatusCode.Accepted => new AcceptedResult(String.Empty, response),
            System.Net.HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
            System.Net.HttpStatusCode.InternalServerError => new ObjectResult(response) { StatusCode = 500 },
            System.Net.HttpStatusCode.Forbidden => new ObjectResult(response) { StatusCode = 403 },
            System.Net.HttpStatusCode.Locked => new ObjectResult(response) { StatusCode = 423 },
            System.Net.HttpStatusCode.Conflict => new ConflictObjectResult(response),
            System.Net.HttpStatusCode.NoContent => new ObjectResult(response) { StatusCode = 204 },
            _ => new BadRequestObjectResult(response)
        };
    }
}
