using Araboon.Core.Behaviors;
using Araboon.Data.Helpers.Resolvers.ChaptersResolver;
using Araboon.Data.Helpers.Resolvers.Mangas;
using Araboon.Data.Helpers.Resolvers.MangasResolver;
using Araboon.Infrastructure.Resolvers.ChaptersResolver;
using Araboon.Infrastructure.Resolvers.MangasResolver;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Araboon.Core
{
    public static class ModuleCoreServices
    {
        public static IServiceCollection AddModuleCoreServices(this IServiceCollection services)
        {
            // Configuration of Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            // Configuation of AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(new[]
                {
                    typeof(ModuleCoreServices).Assembly,
                    typeof(ChapterDateFormatResolver).Assembly,
                    typeof(IsFavoriteResolver).Assembly
                });
            });
            services.AddTransient<ChapterDateFormatResolver>();
            services.AddTransient<IsFavoriteResolver>();
            services.AddTransient<IsCompletedReadingResolver>();
            services.AddTransient<IsReadingLaterResolver>();
            services.AddTransient<IsCurrentlyReadingResolver>();
            services.AddTransient<IsNotificationResolver>();
            services.AddTransient<IsViewResolver>();
            services.AddTransient<ChapterIsArabicResolver>();
            services.AddTransient<MangaDateFormatResolver>();
            services.AddTransient<CommentsCountResolver>();
            // Get Validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
