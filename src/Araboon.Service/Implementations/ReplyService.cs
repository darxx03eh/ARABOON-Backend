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
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Araboon.Service.Implementations
{
    public class ReplyService : IReplyService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly AraboonDbContext context;
        private readonly ILogger<ReplyService> logger;

        public ReplyService(
            IUnitOfWork unitOfWork,
            UserManager<AraboonUser> userManager,
            AraboonDbContext context,
            ILogger<ReplyService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
        }

        public async Task<string> AddLikeAsync(int id)
        {
            logger.LogInformation("Adding like to reply - إضافة لايك للرد | ReplyId: {Id}", id);

            var reply = await unitOfWork.ReplyRepository.GetByIdAsync(id);
            if (reply is null)
            {
                logger.LogWarning("Reply not found - الرد غير موجود | ReplyId: {Id}", id);
                return "ReplyNotFound";
            }

            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                return "UserNotFound";
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود | UserId: {User}", userId);
                return "UserNotFound";
            }

            var usersLikesReply = await context.ReplyLikes
                .Where(r => r.ReplyId.Equals(id))
                .Select(x => x.UserId)
                .ToListAsync();

            if (usersLikesReply.Contains(user.Id))
            {
                logger.LogInformation("User already liked this reply - المستخدم سبق أن ضغط لايك لهذا الرد");
                return "YouAreAlreadyAddedLikeToThisReply";
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var like = await unitOfWork.ReplyLikesRepository.AddAsync(new ReplyLikes()
                {
                    ReplyId = id,
                    UserId = user.Id
                });

                if (like is null)
                {
                    logger.LogError("Failed to add like - فشل إضافة لايك");
                    await transaction.RollbackAsync();
                    return "TheLikeProcessForThisReplyFailed";
                }

                reply.Likes++;
                await unitOfWork.ReplyRepository.UpdateAsync(reply);

                await transaction.CommitAsync();

                logger.LogInformation("Like added successfully - تم إضافة اللايك بنجاح");
                return "TheLikeHasBeenAddedToTheReplySuccessfully";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding like - خطأ أثناء إضافة اللايك");
                await transaction.RollbackAsync();
                return "AnErrorOccurredWhileAddingALikeToTheReply";
            }
        }

        public async Task<(string, GetCommentRepliesResponse?)> AddReplyAsync(string content, int commentId, int toUserId)
        {
            logger.LogInformation("Adding reply - إضافة رد | CommentId: {CommentId}, ToUserId: {ToUser}", commentId, toUserId);

            var comment = await unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment is null)
            {
                logger.LogWarning("Comment not found - التعليق غير موجود");
                return ("CommentNotFound", null);
            }

            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not logged in - المستخدم غير مسجل");
                return ("UserNotFound", null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return ("UserNotFound", null);
            }

            var toUser = await userManager.FindByIdAsync(toUserId.ToString());
            if (toUser is null)
            {
                logger.LogWarning("ToUser not found - المستخدم المردود عليه غير موجود");
                return ("TheUserYouWantToReplyToNotFound", null);
            }

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
                {
                    logger.LogError("Failed to add reply - فشل إضافة الرد");
                    return ("AnErrorOccurredWhileRepling", null);
                }

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

                logger.LogInformation("Reply added successfully - تم إضافة الرد بنجاح");
                return ("ReplyCompletedSuccessfully", response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddReplyAsync - خطأ أثناء إضافة الرد");
                return ("AnErrorOccurredWhileRepling", null);
            }
        }

        public async Task<string> DeleteLikeAsync(int id)
        {
            logger.LogInformation("Deleting like from reply - إزالة لايك من الرد | ReplyId: {Id}", id);

            var reply = await unitOfWork.ReplyRepository.GetByIdAsync(id);
            if (reply is null)
            {
                logger.LogWarning("Reply not found - الرد غير موجود");
                return "ReplyNotFound";
            }

            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                return "UserNotFound";
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return "UserNotFound";
            }

            var usersLikesReply = await context.ReplyLikes
                .Where(r => r.ReplyId.Equals(id))
                .Select(x => x.UserId)
                .ToListAsync();

            if (!usersLikesReply.Contains(user.Id))
            {
                logger.LogInformation("User did not like this reply - المستخدم لم يقم بعمل لايك لهذا الرد");
                return "YouAreNotLikedThisReply";
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var like = await context.ReplyLikes
                    .Where(l => l.UserId.Equals(user.Id) && l.ReplyId.Equals(id))
                    .FirstOrDefaultAsync();

                await unitOfWork.ReplyLikesRepository.DeleteAsync(like);

                reply.Likes--;
                await unitOfWork.ReplyRepository.UpdateAsync(reply);

                await transaction.CommitAsync();

                logger.LogInformation("Like deleted successfully - تم إزالة اللايك بنجاح");
                return "TheLikeHasBeenDeletedFromTheReplySuccessfully";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting like - خطأ أثناء حذف اللايك");
                await transaction.RollbackAsync();
                return "AnErrorOccurredWhileRemovingALikeFromTheReply";
            }
        }

        public async Task<string> DeleteReplyAsync(int id)
        {
            logger.LogInformation("Deleting reply - حذف الرد | ReplyId: {Id}", id);

            var reply = await unitOfWork.ReplyRepository.GetByIdAsync(id);
            if (reply is null)
            {
                logger.LogWarning("Reply not found - الرد غير موجود");
                return "ReplyNotFound";
            }

            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not logged in - المستخدم غير مسجل دخول");
                return "UserNotFound";
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return "UserNotFound";
            }

            var userRole = await userManager.GetRolesAsync(user);

            if (!reply.FromUserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
            {
                logger.LogWarning("Unauthorized delete attempt - المستخدم ليس صاحب الرد وليس أدمن");
                return "YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin";
            }

            var likes = await unitOfWork.ReplyLikesRepository.GetTableNoTracking()
                .Where(l => l.ReplyId.Equals(id))
                .ToListAsync();

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                if (likes != null)
                    await unitOfWork.ReplyLikesRepository.DeleteRangeAsync(likes);

                await unitOfWork.ReplyRepository.DeleteAsync(reply);

                await transaction.CommitAsync();

                logger.LogInformation("Reply deleted successfully - تم حذف الرد بنجاح");
                return "TheReplyHasBeenSuccessfullyDeleted";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting reply - خطأ أثناء حذف الرد");
                await transaction.RollbackAsync();
                return "AnErrorOccurredWhileDeletingTheReply";
            }
        }

        public async Task<(string, string?, string?)> UpdateReplyAsync(string content, int id)
        {
            logger.LogInformation("Updating reply - تعديل الرد | ReplyId: {Id}", id);

            var reply = await unitOfWork.ReplyRepository.GetByIdAsync(id);
            if (reply is null)
            {
                logger.LogWarning("Reply not found - الرد غير موجود");
                return ("ReplyNotFound", null, null);
            }

            var userId = unitOfWork.ReplyRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return ("UserNotFound", null, null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return ("UserNotFound", null, null);
            }

            var roles = await userManager.GetRolesAsync(user);
            if (!reply.FromUserID.Equals(user.Id))
            {
                logger.LogWarning("User unauthorized to update - المستخدم لا يملك صلاحية التعديل");
                return ("YouAreNotTheOwnerOfThisReply", null, null);
            }

            try
            {
                reply.Content = content;
                reply.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.ReplyRepository.UpdateAsync(reply);

                var since = unitOfWork.ReplyRepository.IsArabic()
                    ? reply.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                    : reply.UpdatedAt.Humanize(culture: new CultureInfo("en"));

                logger.LogInformation("Reply updated successfully - تم تعديل الرد بنجاح");

                return ("TheReplyHasBeenSuccessfullyUpdated", content, since);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating reply - خطأ أثناء تعديل الرد");
                return ("AnErrorOccurredWhileUpdatingTheReply", null, null);
            }
        }
    }
}