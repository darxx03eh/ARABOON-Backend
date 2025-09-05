using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Araboon.Service.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;

        public CommentService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<string> AddCommentAsync(string content, int mangaId)
        {
            var userId = unitOfWork.CommentRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
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
    }
}
