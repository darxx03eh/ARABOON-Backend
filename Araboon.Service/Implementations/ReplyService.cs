using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Ocsp;
using System.Globalization;

namespace Araboon.Service.Implementations
{
    public class ReplyService : IReplyService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;

        public ReplyService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<(string, GetCommentRepliesResponse?)> AddReplyAsync(string content, int commentId)
        {
            var comment = await unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment is null)
                return ("CommentNotFound", null);
            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null);
            try
            {
                var result = await unitOfWork.ReplyRepository.AddAsync(new Reply()
                {
                    Content = content,
                    CommentID = commentId,
                    UserID = user.Id
                });
                if (result is null)
                    return ("AnErrorOccurredWhileRepling", null);
                var userOwnComment = await userManager.FindByIdAsync(comment.UserID.ToString());
                var response = new GetCommentRepliesResponse()
                {
                    From = new FromUser()
                    {
                        Id = user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        UserName = user.UserName,
                        To = new ToUser()
                        {
                            Id = userOwnComment.Id,
                            Name = $"{userOwnComment.FirstName} {userOwnComment.LastName}",
                            UserName = userOwnComment.UserName
                        },
                        Reply = new UserReply()
                        {
                            Id = result.ReplyID,
                            Content = result.Content,
                            Since = unitOfWork.ReplyRepository.IsArabic()
                                    ? result.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                                    : result.UpdatedAt.Humanize(culture: new CultureInfo("en")),
                            Likes = 0,
                            IsLike = false
                        }
                    }
                };
                return ("ReplyCompletedSuccessfully", response);
            }
            catch (Exception exp)
            {
                return ("AnErrorOccurredWhileRepling", null);
            }
        }
    }
}
