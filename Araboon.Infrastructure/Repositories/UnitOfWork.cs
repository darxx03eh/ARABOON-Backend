using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;

namespace Araboon.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UnitOfWork(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            CategoryRepository = new CategoryRepository(context, httpContextAccessor);
            ChapterRepository = new ChapterRepository(context, httpContextAccessor);
            ChapterViewRepository = new ChapterViewRepository(context, httpContextAccessor);
            CompletedReadsRepository = new CompletedReadsRepository(context, httpContextAccessor);
            CurrentlyReadingRepository = new CurrentlyReadingRepository(context, httpContextAccessor);
            FavoriteRepository = new FavoriteRepository(context, httpContextAccessor);
            MangaRepository = new MangaRepository(context, httpContextAccessor);
            NotificationsRepository = new NotificationsRepository(context, httpContextAccessor);
            ReadingLaterRepository = new ReadingLaterRepository(context, httpContextAccessor);
            RefreshTokenRepository = new RefreshTokenRepository(context, httpContextAccessor);
            UserRepository = new UserRepository(context, httpContextAccessor);
            CommentRepository = new CommentRepository(context, httpContextAccessor);
            CommentLikesRepository = new CommentLikesRepository(context, httpContextAccessor);
            ReplyRepository = new ReplyRepository(context, httpContextAccessor);
        }
        public ICategoryRepository CategoryRepository { get; set; }
        public IChapterRepository ChapterRepository { get; set; }
        public IChapterViewRepository ChapterViewRepository { get; set; }
        public ICompletedReadsRepository CompletedReadsRepository { get; set; }
        public ICurrentlyReadingRepository CurrentlyReadingRepository { get; set; }
        public IFavoriteRepository FavoriteRepository { get; set; }
        public IMangaRepository MangaRepository { get; set; }
        public INotificationsRepository NotificationsRepository { get; set; }
        public IReadingLaterRepository ReadingLaterRepository { get; set; }
        public IRefreshTokenRepository RefreshTokenRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public ICommentRepository CommentRepository { get; set; }
        public ICommentLikesRepository CommentLikesRepository { get; set; }
        public IReplyRepository ReplyRepository { get; set; }
    }
}
