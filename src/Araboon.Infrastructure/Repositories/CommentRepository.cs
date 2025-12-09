using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Araboon.Infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public CommentRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager) 
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<(string, PaginatedResult<GetCommentRepliesResponse>?)> GetCommentRepliesAsync(int id, int pageNumber, int pageSize)
        {
            var comment = await GetTableNoTracking().Where(c => c.CommentID.Equals(id)).FirstOrDefaultAsync();
            if (comment is null)
                return ("CommentNotFound", null);
            var replyQueryable = context.Replies.AsNoTracking().Where(c => c.CommentID.Equals(id))
                .OrderBy(c => c.UpdatedAt).AsQueryable();
            if (replyQueryable is null)
                return ("RepliesNotFound", null);
            string? userId = ExtractUserIdFromToken();
            IList<int> likes = new List<int>();
            if (!string.IsNullOrWhiteSpace(userId))
                likes = await context.ReplyLikes.Where(x => x.UserId.Equals(int.Parse(userId))).Select(x => x.ReplyId).ToListAsync();

            var pagedReplies = await replyQueryable.ToPaginatedListAsync(pageNumber, pageSize);
            if (pagedReplies.Data.Equals(0))
                return ("RepliesNotFound", null);

            var replies = pagedReplies.Data.Select(x => new GetCommentRepliesResponse()
            {
                Id = x.ReplyID,
                Content = x.Content,
                Since = IsArabic()
                        ? x.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                        : x.UpdatedAt.Humanize(culture: new CultureInfo("en")),
                Likes = x.Likes,
                IsLiked = likes.Contains(x.ReplyID),
                User = new FromUser()
                {
                    Id = x.FromUserID,
                    Name = $"{x.FromUser.FirstName} {x.FromUser.LastName}",
                    UserName = x.FromUser.UserName,
                    ProfileImage = new Araboon.Data.Response.Users.Queries.ProfileImage()
                    {
                        OriginalImage = x.FromUser.ProfileImage.OriginalImage,
                        CropData = new Araboon.Data.Response.Users.Queries.CropData()
                        {
                            Position = new Araboon.Data.Response.Users.Queries.Position()
                            {
                                X = x.FromUser.ProfileImage.X,
                                Y = x.FromUser.ProfileImage.Y,
                            },
                            Scale = x.FromUser.ProfileImage.Scale,
                            Rotate = x.FromUser.ProfileImage.Rotate,
                        },
                    },
                },
                ReplyToUser = new ToUser()
                {
                    Id = x.ToUserID,
                    Name = $"{x.ToUser.FirstName} {x.ToUser.LastName}",
                    UserName = x.ToUser.UserName
                }
            }).ToList();
            var result = PaginatedResult<GetCommentRepliesResponse>.Success(
                replies,
                pagedReplies.TotalCount,
                pagedReplies.TotalPages,
                pagedReplies.PageSize
                );
            result.CurrentPage = pagedReplies.CurrentPage;
            if (result.Data.Count.Equals(0))
                return ("RepliesNotFound", null);
            return ("RepliesFound", result);
        }

        public async Task<(string, int?)> GetCommentsCountForMangaAsync(int id)
        {
            var manga = await context.Mangas.Where(manga => manga.MangaID.Equals(id)).FirstOrDefaultAsync();
            if (manga is null)
                return ("MangaNotFound", null);

            var commentsCount = await GetTableNoTracking().Where(comment => comment.MangaID.Equals(id)).CountAsync();
            if (commentsCount.Equals(0))
                return ("CommentsNotFound", null);

            return ("CommentsFound", commentsCount);
        }
    }
}
