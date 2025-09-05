using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
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
    }
}
