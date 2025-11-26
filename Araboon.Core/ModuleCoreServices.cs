using Araboon.Core.Behaviors;
using Araboon.Core.ResponseHelper;
using Araboon.Core.Translations;
using Araboon.Data.Helpers.Resolvers.ChaptersResolver;
using Araboon.Data.Helpers.Resolvers.Mangas;
using Araboon.Data.Helpers.Resolvers.MangasResolver;
using Araboon.Infrastructure.Resolvers.ChaptersResolver;
using Araboon.Infrastructure.Resolvers.MangasResolver;
using Araboon.Infrastructure.Resolvers.SwipersResolver;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.RateLimiting;

namespace Araboon.Core
{
    public static class ModuleCoreServices
    {
        public static IServiceCollection AddModuleCoreServices(this IServiceCollection services)
        {
            // Configuration of Mediator
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            #region AutoMapper Settings

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

            #endregion AutoMapper Settings

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
            services.AddTransient<IsEnglishAvilableResolver>();
            services.AddTransient<IsArabicAvilableResolver>();
            services.AddTransient<SwiperDateFormatResolver>();
            // Get Validators

            #region RateLimiter Settings

            services.AddRateLimiter(options =>
            {
                options.AddPolicy("SendForgetPasswordEmail", context =>
                {
                    var email = context.Request.Headers["Rate-Limiting-Key"].ToString();
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: string.IsNullOrWhiteSpace(email) ? context.Connection.RemoteIpAddress.ToString() : email,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromHours(2),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }
                    );
                });
                options.RejectionStatusCode = 429;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    await RateLimiterHelper.HandleRejectedAsync(
                        context,
                        SharedTranslationKeys.YouHaveExceededTheLimitForSendingPasswordResetEmailsPleaseTryAgainLater
                    );
                };
            });

            services.AddRateLimiter(options =>
            {
                options.AddPolicy("SendConfirmationEmail", context =>
                {
                    var username = context.Request.Headers["Rate-Limiting-Key"].ToString();
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: string.IsNullOrWhiteSpace(username) ? context.Connection.RemoteIpAddress.ToString() : username,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromHours(2),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }
                    );
                });
                options.RejectionStatusCode = 429;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    await RateLimiterHelper.HandleRejectedAsync(
                        context,
                        SharedTranslationKeys.YouHaveExceededTheLimitForSendingConfirmationEmailsPleaseTryAgainLater
                    );
                };
            });

            services.AddRateLimiter(options =>
            {
                options.AddPolicy("Login", context =>
                {
                    var loginSuccess = context.Items["LoginFailed"] as bool?;
                    if (loginSuccess == false)
                        return RateLimitPartition.GetNoLimiter("LoginFailed");
                    var username = context.Request.Headers["Rate-Limiting-Key"].ToString();
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: string.IsNullOrWhiteSpace(username) ? context.Connection.RemoteIpAddress.ToString() : username,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(15),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }
                    );
                });
                options.RejectionStatusCode = 429;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    await RateLimiterHelper.HandleRejectedAsync(
                        context,
                        SharedTranslationKeys.YouHaveExceededTheLimitForSendingLoginRequestPleaseTryAgainLater
                    );
                };
            });

            #endregion RateLimiter Settings

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}