using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Araboon.Service
{
    public static class ModuleServiceServices
    {
        public static IServiceCollection AddModuleServiceServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IAuthenticationService), typeof(AuthenticationService));
            services.AddTransient(typeof(ITokenService), typeof(TokenService));
            services.AddTransient(typeof(IEmailService), typeof(EmailService));
            services.AddTransient(typeof(IAvatarService), typeof(AvatarService));
            services.AddTransient(typeof(ICloudinaryService), typeof(CloudinaryService));
            services.AddTransient(typeof(ICategoryService), typeof(CategoryService));
            services.AddTransient(typeof(IChapterService), typeof(ChapterService));
            services.AddTransient(typeof(IChapterViewService), typeof(ChapterViewService));
            services.AddTransient(typeof(ICompletedReadsService), typeof(CompletedReadsService));
            services.AddTransient(typeof(ICurrentlyReadingService), typeof(CurrentlyReadingService));
            services.AddTransient(typeof(IFavoriteService), typeof(FavoriteService));
            services.AddTransient(typeof(IMangaService), typeof(MangaService));
            services.AddTransient(typeof(INotificationsService), typeof(NotificationsService));
            services.AddTransient(typeof(IReadingLaterService), typeof(ReadingLaterService));
            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddTransient(typeof(ICommentService), typeof(CommentService));
            services.AddTransient(typeof(IReplyService), typeof(ReplyService));
            services.AddTransient(typeof(IRatingService), typeof(RatingService));
            services.AddTransient(typeof(IChapterImagesService), typeof(ChapterImagesService));
            services.AddTransient(typeof(ISwiperService), typeof(SwiperService));
            return services;
        }
    }
}
