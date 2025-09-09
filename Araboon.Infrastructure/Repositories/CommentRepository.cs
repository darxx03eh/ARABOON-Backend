using Araboon.Data.Entities;
using Araboon.Data.Response.Comments.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Araboon.Infrastructure.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CommentRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
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

            var userOwnsId = await context.Comments.AsTracking().Where(c => c.CommentID.Equals(id)).Select(c => c.UserID).FirstOrDefaultAsync();
            var user = await context.Users.FindAsync(userOwnsId);
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
                    Id = x.UserID,
                    Name = $"{x.User.FirstName} {x.User.LastName}",
                    UserName = x.User.UserName,
                    ProfileImage = new Araboon.Data.Response.Users.Queries.ProfileImage()
                    {
                        OriginalImage = x.User.ProfileImage.OriginalImage,
                        CropData = new Araboon.Data.Response.Users.Queries.CropData()
                        {
                            Position = new Araboon.Data.Response.Users.Queries.Position()
                            {
                                X = x.User.ProfileImage.X,
                                Y = x.User.ProfileImage.Y,
                            },
                            Scale = x.User.ProfileImage.Scale,
                            Rotate = x.User.ProfileImage.Rotate,
                        },
                    },
                },
                ReplyToUser = new ToUser()
                {
                    Id = user.Id,
                    Name = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName
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
    }
}
