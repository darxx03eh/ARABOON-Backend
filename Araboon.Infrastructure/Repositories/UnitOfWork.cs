using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Araboon.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public UnitOfWork(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            CategoryRepository = new CategoryRepository(context, httpContextAccessor, userManager);
            ChapterRepository = new ChapterRepository(context, httpContextAccessor, userManager);
            ChapterViewRepository = new ChapterViewRepository(context, httpContextAccessor, userManager);
            CompletedReadsRepository = new CompletedReadsRepository(context, httpContextAccessor, userManager);
            CurrentlyReadingRepository = new CurrentlyReadingRepository(context, httpContextAccessor, userManager);
            FavoriteRepository = new FavoriteRepository(context, httpContextAccessor, userManager);
            MangaRepository = new MangaRepository(context, httpContextAccessor, userManager);
            NotificationsRepository = new NotificationsRepository(context, httpContextAccessor, userManager);
            ReadingLaterRepository = new ReadingLaterRepository(context, httpContextAccessor, userManager);
            RefreshTokenRepository = new RefreshTokenRepository(context, httpContextAccessor, userManager);
            UserRepository = new UserRepository(context, httpContextAccessor, userManager);
            CommentRepository = new CommentRepository(context, httpContextAccessor, userManager);
            CommentLikesRepository = new CommentLikesRepository(context, httpContextAccessor, userManager);
            ReplyRepository = new ReplyRepository(context, httpContextAccessor, userManager);
            ReplyLikesRepository = new ReplyLikesRepository(context, httpContextAccessor, userManager);
            RatingsRepository = new RatingsRepository(context, httpContextAccessor, userManager);
            CategoryMangaRepository = new CategoryMangaRepository(context, httpContextAccessor, userManager);
            ArabicChapterImagesRepository = new ArabicChapterImagesRepository(context, httpContextAccessor, userManager);
            EnglishChapterImagesRepository = new EnglishChapterImagesRepository(context, httpContextAccessor, userManager);
            SwiperRepository = new SwiperRepository(context, httpContextAccessor, userManager);
        }
        public IArabicChapterImagesRepository ArabicChapterImagesRepository { get; set; }
        public IEnglishChapterImagesRepository EnglishChapterImagesRepository { get; set; }
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
        public IReplyLikesRepository ReplyLikesRepository { get; set; }
        public IRatingsRepository RatingsRepository { get; set; }
        public ICategoryMangaRepository CategoryMangaRepository { get; set; }
        public ISwiperRepository SwiperRepository { get; set; }
    }
}
