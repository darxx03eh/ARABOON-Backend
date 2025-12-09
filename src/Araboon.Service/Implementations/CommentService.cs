using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Araboon.Service.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly AraboonDbContext context;
        private readonly ILogger<CommentService> logger;

        public CommentService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager, AraboonDbContext context, ILogger<CommentService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
        }

        public async Task<(string, GetMangaCommentsResponse?)> AddCommentAsync(string content, int mangaId)
        {
            logger.LogInformation("Adding new comment - إضافة تعليق جديد | MangaId: {MangaId}", mangaId);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                return ("MangaNotFound", null);
            }

            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                return ("UserNotFound", null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود | UserId: {UserId}", userId);
                return ("UserNotFound", null);
            }

            try
            {
                var result = await unitOfWork.CommentRepository.AddAsync(new Comment()
                {
                    Content = content,
                    MangaID = mangaId,
                    UserID = user.Id
                });

                if (result is null)
                {
                    logger.LogError("Failed to add comment - فشل في إضافة التعليق");
                    return ("AnErrorOccurredWhileCommenting", null);
                }

                var comment = new GetMangaCommentsResponse()
                {
                    Id = result.CommentID,
                    replyCount = 0,
                    Content = result.Content,
                    Since = unitOfWork.CommentRepository.IsArabic()
                        ? result.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                        : result.UpdatedAt.Humanize(culture: new CultureInfo("en")),
                    Likes = 0,
                    IsLiked = false,
                    User = new User()
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
                    }
                };

                logger.LogInformation("Comment added successfully - تم إضافة التعليق بنجاح | CommentId: {Id}", result.CommentID);
                return ("CommentCompletedSuccessfully", comment);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding comment - خطأ أثناء إضافة التعليق");
                return ("AnErrorOccurredWhileCommenting", null);
            }
        }

        public async Task<string> AddLikeAsync(int id)
        {
            logger.LogInformation("Adding like to comment - إضافة إعجاب للتعليق | CommentId: {Id}", id);

            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                logger.LogWarning("Comment not found - التعليق غير موجود | CommentId: {Id}", id);
                return "CommentNotFound";
            }

            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                return "UserNotFound";
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود | UserId: {UserId}", userId);
                return "UserNotFound";
            }

            var usersLikesComment = await context.CommentLikes
                .Where(c => c.CommentId.Equals(id))
                .Select(u => u.UserId)
                .ToListAsync();

            if (usersLikesComment.Contains(user.Id))
            {
                logger.LogInformation("User already liked this comment - المستخدم قام بالإعجاب مسبقًا | CommentId: {Id}, UserId: {UserId}", id, user.Id);
                return "YouAreAlreadyAddedLikeToThisComment";
            }

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var like = await unitOfWork.CommentLikesRepository.AddAsync(new CommentLikes()
                    {
                        CommentId = id,
                        UserId = user.Id
                    });

                    if (like is null)
                    {
                        logger.LogError("Failed to add like - فشل إضافة الإعجاب | CommentId: {Id}", id);
                        await transaction.RollbackAsync();
                        return "TheLikeProcessForThisCommentFailed";
                    }

                    comment.Likes += 1;
                    await unitOfWork.CommentRepository.UpdateAsync(comment);
                    await transaction.CommitAsync();

                    logger.LogInformation("Like added successfully - تم إضافة الإعجاب بنجاح | CommentId: {Id}", id);
                    return "TheLikeHasBeenAddedToTheCommentSuccessfully";
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error adding like - خطأ أثناء إضافة الإعجاب | CommentId: {Id}", id);
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileAddingALikeToTheComment";
                }
            }
        }

        public async Task<string> DeleteCommentAsync(int id)
        {
            logger.LogInformation("Deleting comment - حذف التعليق | CommentId: {Id}", id);

            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                logger.LogWarning("Comment not found - التعليق غير موجود | CommentId: {Id}", id);
                return "CommentNotFound";
            }

            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                return "UserNotFound";
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود | UserId: {UserId}", userId);
                return "UserNotFound";
            }

            var userRole = await userManager.GetRolesAsync(user);
            if (!comment.UserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
            {
                logger.LogWarning("Unauthorized delete attempt - محاولة حذف غير مصرح بها | CommentId: {Id}, UserId: {UserId}", id, userId);
                return "YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin";
            }

            var replies = await unitOfWork.ReplyRepository.GetTableNoTracking().Where(r => r.CommentID.Equals(id)).ToListAsync();
            var likes = await unitOfWork.CommentLikesRepository.GetTableNoTracking().Where(l => l.CommentId.Equals(id)).ToListAsync();

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (replies is not null)
                        await unitOfWork.ReplyRepository.DeleteRangeAsync(replies);

                    if (likes is not null)
                        await unitOfWork.CommentLikesRepository.DeleteRangeAsync(likes);

                    await unitOfWork.CommentRepository.DeleteAsync(comment);
                    await transaction.CommitAsync();

                    logger.LogInformation("Comment deleted successfully - تم حذف التعليق بنجاح | CommentId: {Id}", id);
                    return "TheCommentHasBeenSuccessfullyDeleted";
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deleting comment - خطأ أثناء حذف التعليق | CommentId: {Id}", id);

                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();

                    return "AnErrorOccurredWhileDeletingTheComment";
                }
            }
        }

        public async Task<string> DeleteLikeAsync(int id)
        {
            logger.LogInformation("Removing like from comment - إزالة الإعجاب من التعليق | CommentId: {Id}", id);

            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                logger.LogWarning("Comment not found - التعليق غير موجود | CommentId: {Id}", id);
                return "CommentNotFound";
            }

            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found in token - المستخدم غير موجود في التوكن");
                return "UserNotFound";
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود | UserId: {UserId}", userId);
                return "UserNotFound";
            }

            var usersLikesComment = await context.CommentLikes
                .Where(c => c.CommentId.Equals(id))
                .Select(u => u.UserId)
                .ToListAsync();

            if (!usersLikesComment.Contains(user.Id))
            {
                logger.LogWarning("User has not liked the comment - المستخدم لم يعمل إعجاب | CommentId: {Id}, UserId: {UserId}", id, user.Id);
                return "YouAreNotLikedThisComment";
            }

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var like = await context.CommentLikes
                        .Where(l => l.UserId.Equals(user.Id) && l.CommentId.Equals(id))
                        .FirstOrDefaultAsync();

                    await unitOfWork.CommentLikesRepository.DeleteAsync(like);

                    comment.Likes -= 1;
                    await unitOfWork.CommentRepository.UpdateAsync(comment);

                    await transaction.CommitAsync();

                    logger.LogInformation("Like removed successfully - تم حذف الإعجاب بنجاح | CommentId: {Id}", id);
                    return "TheLikeHasBeenDeletedFromTheCommentSuccessfully";
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error deleting like - خطأ أثناء حذف الإعجاب | CommentId: {Id}", id);

                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();

                    return "AnErrorOccurredWhileRemovingALikeFromTheComment";
                }
            }
        }

        public async Task<(string, string?, string?)> UpdateCommentAsync(string content, int id)
        {
            logger.LogInformation("Updating comment - تعديل التعليق | CommentId: {Id}", id);

            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
            {
                logger.LogWarning("Comment not found - التعليق غير موجود | CommentId: {Id}", id);
                return ("CommentNotFound", null, null);
            }

            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found in token - المستخدم غير موجود");
                return ("UserNotFound", null, null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found - المستخدم غير موجود | UserId: {UserId}", userId);
                return ("UserNotFound", null, null);
            }

            var userRole = await userManager.GetRolesAsync(user);
            if (!comment.UserID.Equals(user.Id))
            {
                logger.LogWarning("Unauthorized update attempt - محاولة تعديل غير مصرح بها | CommentId: {Id}, UserId: {UserId}", id, userId);
                return ("YouAreNotTheOwnerOfThisComment", null, null);
            }

            try
            {
                comment.Content = content;
                comment.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.CommentRepository.UpdateAsync(comment);

                var since = unitOfWork.CommentRepository.IsArabic()
                    ? comment.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                    : comment.UpdatedAt.Humanize(culture: new CultureInfo("en"));

                logger.LogInformation("Comment updated successfully - تم تعديل التعليق | CommentId: {Id}", id);
                return ("TheCommentHasBeenSuccessfullyUpdated", content, since);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating comment - خطأ أثناء تعديل التعليق | CommentId: {Id}", id);
                return ("AnErrorOccurredWhileUpdatingTheComment", null, null);
            }
        }

        public async Task<(string, PaginatedResult<GetCommentRepliesResponse>?)> GetCommentRepliesAsync(int id, int pageNumber, int pageSize)
        {
            logger.LogInformation("Fetching comment replies - جلب ردود التعليق | CommentId: {Id}", id);

            var (result, replies) = await unitOfWork.CommentRepository.GetCommentRepliesAsync(id, pageNumber, pageSize);

            return result switch
            {
                "CommentNotFound" => ("CommentNotFound", null),
                "RepliesNotFound" => ("RepliesNotFound", null),
                "RepliesFound" => ("RepliesFound", replies),
                _ => ("RepliesNotFound", null)
            };
        }
    }
}