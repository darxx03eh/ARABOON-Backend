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
using System.Globalization;

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

        public async Task<(string, GetMangaCommentsResponse?)> AddCommentAsync(string content, int mangaId)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return ("MangaNotFound", null);
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null);
            try
            {
                var result = await unitOfWork.CommentRepository.AddAsync(new Comment()
                {
                    Content = content,
                    MangaID = mangaId,
                    UserID = user.Id
                });
                if (result is null)
                    return ("AnErrorOccurredWhileCommenting", null);
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
                return ("CommentCompletedSuccessfully", comment);
            }
            catch (Exception exp)
            {
                return ("AnErrorOccurredWhileCommenting", null);
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
                    return "TheCommentHasBeenSuccessfullyDeleted";

                }
                catch (Exception exp)
                {
                    if(transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileDeletingTheComment";
                }
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

        public async Task<(string, string?, string?)> UpdateCommentAsync(string content, int id)
        {
            var comment = await unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment is null)
                return ("CommentNotFound", null, null);
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null, null);
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null, null);
            var userRole = await userManager.GetRolesAsync(user);
            if (!comment.UserID.Equals(user.Id) && !userRole.Contains(Roles.Admin))
                return ("YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin", null, null);
            try
            {
                comment.Content = content;
                comment.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.CommentRepository.UpdateAsync(comment);
                var since = unitOfWork.CommentRepository.IsArabic()
                                    ? comment.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                                    : comment.UpdatedAt.Humanize(culture: new CultureInfo("en"));
                return ("TheCommentHasBeenSuccessfullyUpdated", content, since);

            }
            catch (Exception exp)
            {
                return ("AnErrorOccurredWhileUpdatingTheComment", null, null);
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
