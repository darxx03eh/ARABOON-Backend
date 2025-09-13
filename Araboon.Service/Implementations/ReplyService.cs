using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace Araboon.Service.Implementations
{
    public class ReplyService : IReplyService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly AraboonDbContext context;

        public ReplyService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager, AraboonDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<string> AddLikeAsync(int id)
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

            var usersLikesReply = await context.ReplyLikes.Where(reply => reply.ReplyId.Equals(id))
                                    .Select(user => user.UserId).ToListAsync();
            if (usersLikesReply.Contains(user.Id))
                return "YouAreAlreadyAddedLikeToThisReply";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var like = await unitOfWork.ReplyLikesRepository.AddAsync(new ReplyLikes()
                    {
                        ReplyId = id,
                        UserId = user.Id
                    });
                    if (like is null)
                    {
                        await transaction.RollbackAsync();
                        return "TheLikeProcessForThisReplyFailed";
                    }
                    reply.Likes += 1;
                    await unitOfWork.ReplyRepository.UpdateAsync(reply);
                    await transaction.CommitAsync();
                    return "TheLikeHasBeenAddedToTheReplySuccessfully";
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection is null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileAddingALikeToTheReply";
                }
            }
        }

        public async Task<(string, GetCommentRepliesResponse?)> AddReplyAsync(string content, int commentId, int toUserId)
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
            var toUser = await userManager.FindByIdAsync(toUserId.ToString());
            if (toUser is null)
                return ("TheUserYouWantToReplyToNotFound", null);
            try
            {
                var result = await unitOfWork.ReplyRepository.AddAsync(new Reply()
                {
                    Content = content,
                    CommentID = commentId,
                    FromUserID = user.Id,
                    ToUserID = toUserId
                });
                if (result is null)
                    return ("AnErrorOccurredWhileRepling", null);
                
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
                        Id = toUser.Id,
                        Name = $"{toUser.FirstName} {toUser.LastName}",
                        UserName = toUser.UserName
                    }
                };
                return ("ReplyCompletedSuccessfully", response);
            }
            catch (Exception exp)
            {
                return ("AnErrorOccurredWhileRepling", null);
            }
        }

        public async Task<string> DeleteLikeAsync(int id)
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

            var usersLikesReply = await context.ReplyLikes.Where(reply => reply.ReplyId.Equals(id))
                                    .Select(user => user.UserId).ToListAsync();
            if (!usersLikesReply.Contains(user.Id))
                return "YouAreNotLikedThisReply";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var like = await context.ReplyLikes.Where(l => l.UserId.Equals(user.Id) && l.ReplyId.Equals(id)).FirstOrDefaultAsync();
                    await unitOfWork.ReplyLikesRepository.DeleteAsync(like);
                    reply.Likes -= 1;
                    await unitOfWork.ReplyRepository.UpdateAsync(reply);
                    await transaction.CommitAsync();
                    return "TheLikeHasBeenDeletedFromTheReplySuccessfully";
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection is null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileRemovingALikeFromTheReply";
                }
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
            if (!reply.FromUserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
                return "YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin";

            var likes = await unitOfWork.ReplyLikesRepository.GetTableNoTracking().Where(l => l.ReplyId.Equals(id)).ToListAsync();
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (likes is not null)
                        await unitOfWork.ReplyLikesRepository.DeleteRangeAsync(likes);
                    await unitOfWork.ReplyRepository.DeleteAsync(reply);
                    await transaction.CommitAsync();
                    return "TheReplyHasBeenSuccessfullyDeleted";
                }
                catch (Exception exp)
                {
                    if(transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileDeletingTheReply";
                }
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
            if (!reply.FromUserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
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
