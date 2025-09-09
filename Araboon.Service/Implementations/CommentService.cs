using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Araboon.Service.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly AraboonDbContext context;

        public CommentService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager, AraboonDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<string> AddCommentAsync(string content, int mangaId)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            try
            {
                var result = await unitOfWork.CommentRepository.AddAsync(new Comment()
                {
                    Content = content,
                    MangaID = mangaId,
                    UserID = user.Id
                });
                if (result is null)
                    return "AnErrorOccurredWhileCommenting";
                return "CommentCompletedSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileCommenting";
            }
        }

        public async Task<string> AddLikeAsync(int id)
        {
            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
                return "CommentNotFound";
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";

            var usersLikesComment = await context.CommentLikes.Where(comment => comment.CommentId.Equals(id))
                                    .Select(user => user.UserId).ToListAsync();
            if (usersLikesComment.Contains(user.Id))
                return "YouAreAlreadyAddedLikeToThisComment";
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
                        await transaction.RollbackAsync();
                        return "TheLikeProcessForThisCommentFailed";
                    }
                    comment.Likes += 1;
                    await unitOfWork.CommentRepository.UpdateAsync(comment);
                    await transaction.CommitAsync();
                    return "TheLikeHasBeenAddedToTheCommentSuccessfully";
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection is null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileAddingALikeToTheComment";
                }
            }
        }

        public async Task<string> DeleteCommentAsync(int id)
        {
            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
                return "CommentNotFound";
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            var userRole = await userManager.GetRolesAsync(user);
            if (!comment.UserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
                return "YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin";

            try
            {
                await unitOfWork.CommentRepository.DeleteAsync(comment);
                return "TheCommentHasBeenSuccessfullyDeleted";

            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeletingTheComment";
            }
        }

        public async Task<string> DeleteLikeAsync(int id)
        {
            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
                return "CommentNotFound";
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";

            var usersLikesComment = await context.CommentLikes.Where(comment => comment.CommentId.Equals(id))
                                    .Select(user => user.UserId).ToListAsync();
            if (!usersLikesComment.Contains(user.Id))
                return "YouAreNotLikedThisComment";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var like = await context.CommentLikes.Where(l => l.UserId.Equals(user.Id) && l.CommentId.Equals(id)).FirstOrDefaultAsync();
                    await unitOfWork.CommentLikesRepository.DeleteAsync(like);
                    comment.Likes -= 1;
                    await unitOfWork.CommentRepository.UpdateAsync(comment);
                    await transaction.CommitAsync();
                    return "TheLikeHasBeenDeletedFromTheCommentSuccessfully";
                }
                catch (Exception)
                {
                    if (transaction.GetDbTransaction().Connection is null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileRemovingALikeFromTheComment";
                }
            }
        }

        public async Task<string> UpdateCommentAsync(string content, int id)
        {
            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
                return "CommentNotFound";
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            var userRole = await userManager.GetRolesAsync(user);
            if (!comment.UserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
                return "YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin";
            try
            {
                comment.Content = content;
                comment.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.CommentRepository.UpdateAsync(comment);
                return "TheCommentHasBeenSuccessfullyUpdated";

            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileUpdatingTheComment";
            }
        }
        public async Task<(string, PaginatedResult<GetCommentRepliesResponse>?)> GetCommentRepliesAsync(int id, int pageNumber, int pageSize)
        {
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
