using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Identity;
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
                    Id = result.ReplyID,
                    Content = result.Content,
                    Since = unitOfWork.ReplyRepository.IsArabic()
                                    ? result.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                                    : result.UpdatedAt.Humanize(culture: new CultureInfo("en")),
                    Likes = 0,
                    IsLiked = false,
                    User = new FromUser()
                    {
                        Id = user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        UserName = user.UserName,
                        ProfileImage = new Data.Response.Users.Queries.ProfileImage()
                        {
                            OriginalImage = user.ProfileImage.OriginalImage,
                            CropData = new Data.Response.Users.Queries.CropData()
                            {
                                Position = new Data.Response.Users.Queries.Position()
                                {
                                    X = user.ProfileImage.X,
                                    Y = user.ProfileImage.Y
                                },
                                Scale = user.ProfileImage.Scale,
                                Rotate = user.ProfileImage.Rotate
                            }
                        }
                    },
                    ReplyToUser = new ToUser()
                    {
                        Id = userOwnComment.Id,
                        Name = $"{userOwnComment.FirstName} {userOwnComment.LastName}",
                        UserName = userOwnComment.UserName
                    }
                };
                return ("ReplyCompletedSuccessfully", response);
            }
            catch (Exception exp)
            {
                return ("AnErrorOccurredWhileRepling", null);
            }
        }
        public async Task<string> DeleteReplyAsync(int id)
        {
            var reply = await unitOfWork.ReplyRepository.GetByIdAsync(id);
            if (reply is null)
                return "ReplyNotFound";
            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";

            var userRole = await userManager.GetRolesAsync(user);
            if (!reply.UserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
                return "YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin";

            try
            {
                await unitOfWork.ReplyRepository.DeleteAsync(reply);
                return "TheReplyHasBeenSuccessfullyDeleted";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeletingTheReply";
            }
        }

        public async Task<(string, string?, string?)> UpdateReplyAsync(string content, int id)
        {
            var reply = await unitOfWork.ReplyRepository.GetByIdAsync(id);
            if (reply is null)
                return ("ReplyNotFound", null, null);
            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null, null);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null, null);
            var userRole = await userManager.GetRolesAsync(user);
            if (!reply.UserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
                return ("YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin", null, null);
            try
            {
                reply.Content = content;
                reply.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.ReplyRepository.UpdateAsync(reply);
                var since = unitOfWork.ReplyRepository.IsArabic()
                                    ? reply.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                                    : reply.UpdatedAt.Humanize(culture: new CultureInfo("en"));
                return ("TheReplyHasBeenSuccessfullyUpdated", content, since);
            }
            catch(Exception exp)
            {
                return ("AnErrorOccurredWhileUpdatingTheReply", null, null);
            }
        }
    }
}
