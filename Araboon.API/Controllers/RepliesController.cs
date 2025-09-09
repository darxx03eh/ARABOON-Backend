using Araboon.API.Bases;
using Araboon.Core.Features.Replies.Commands.Models;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araboon.API.Controllers
{
    [Authorize]
    [ApiController]
    public class RepliesController : AppBaseController
    {
        [HttpPost(Router.ReplyRouting.AddReply)]
        public async Task<IActionResult> AddReply(AddReplyToCommentCommand request)
        {
            var result = await mediator.Send(request);
            return Result(result);
        }
        [HttpDelete(Router.ReplyRouting.DeleteReply)]
        public async Task<IActionResult> DeleteReply(int id)
        {
            var result = await mediator.Send(new DeleteReplyCommand(id));
            return Result(result);
        }
        [HttpPatch(Router.ReplyRouting.UpdateReply)]
        public async Task<IActionResult> UpdateReply(int id, ReplyDto content)
        {
            var result = await mediator.Send(new UpdateReplyCommand(id) { Content = content.Content });
            return Result(result);
        }
    }
}