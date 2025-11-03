namespace Araboon.Infrastructure.IRepositories
{
    public interface IUnitOfWork
    {
        public IArabicChapterImagesRepository ArabicChapterImagesRepository { get; set; }
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
        public IEnglishChapterImagesRepository EnglishChapterImagesRepository { get; set; }
    }
}
