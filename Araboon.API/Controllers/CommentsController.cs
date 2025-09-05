using Araboon.API.Bases;
using Araboon.Core.Features.Comments.Commands.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [ApiController]
    [Authorize]
    public class CommentsController : AppBaseController
    {
        [HttpPost(Router.CommentRouting.AddComment)]
        public async Task<IActionResult> AddComment(AddCommentCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpDelete(Router.CommentRouting.DeleteComment)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var result = await mediator.Send(new DeleteCommentCommand(id));
            return Result(result);
        }
        [HttpPatch(Router.CommentRouting.UpdateComment)]
        public async Task<IActionResult> UpdateComment(int id, CommentDTO content)
        {
            var result = await mediator.Send(new UpdateCommentCommand(id) { Content = content.Content });
            return Result(result);
        }
    }
}
