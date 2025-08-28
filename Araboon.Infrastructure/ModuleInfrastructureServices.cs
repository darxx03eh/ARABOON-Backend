using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Araboon.Infrastructure
{
    public static class ModuleInfrastructureServices
    {
        public static IServiceCollection AddModuleInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Identity Settings
            services.AddIdentity<AraboonUser, AraboonRole>(options =>
            {
                // Sign in settings
                options.SignIn.RequireConfirmedEmail = true;
                // password settings
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;
                // lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // User settings
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
            }).AddEntityFrameworkStores<AraboonDbContext>().AddDefaultTokenProviders();
            #endregion

            #region Prepare Settings
            var emailSettings = new EmailSettings();
            var cloudinarySettings = new CloudinarySettings();
            var jwtSettings = new JwtSettings();
            var encryptionSettings = new EncryptionSettings();
            configuration.GetSection(nameof(emailSettings)).Bind(emailSettings);
            configuration.GetSection(nameof(cloudinarySettings)).Bind(cloudinarySettings);
            configuration.GetSection(nameof(jwtSettings)).Bind(jwtSettings);
            configuration.GetSection(nameof(encryptionSettings)).Bind(encryptionSettings);
            services.AddSingleton(emailSettings);
            services.AddSingleton(cloudinarySettings);
            services.AddSingleton(jwtSettings);
            services.AddSingleton(encryptionSettings);
            #endregion

            #region Authentication and JWT Settings
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    ValidAudience = jwtSettings.Audience,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    RoleClaimType = nameof(UserClaimModel.Role),
                    NameClaimType = nameof(UserClaimModel.UserName),
                    ClockSkew = TimeSpan.Zero
                };
            });
            #endregion

            #region Dependancy injection
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient(typeof(IRefreshTokenRepository), typeof(RefreshTokenRepository));
            services.AddTransient(typeof(IMangaRepository), typeof(MangaRepository));
            services.AddTransient(typeof(IChapterRepository), typeof(ChapterRepository));
            services.AddTransient(typeof(IFavoriteRepository), typeof(FavoriteRepository));
            services.AddTransient(typeof(ICompletedReadsRepository), typeof(CompletedReadsRepository));
            services.AddTransient(typeof(ICurrentlyReadingRepository), typeof(CurrentlyReadingRepository));
            services.AddTransient(typeof(IReadingLaterRepository), typeof(ReadingLaterRepository));
            services.AddTransient(typeof(INotificationsRepository), typeof(NotificationsRepository));
            services.AddTransient(typeof(IChapterViewRepository), typeof(ChapterViewRepository));
            services.AddTransient(typeof(ICategoryRepository), typeof(CategoryRepository));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IUserRepository), typeof(UserRepository));
            #endregion

            return services;
        }
    }
}
