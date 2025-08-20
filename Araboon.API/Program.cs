
using Araboon.Core;
using Araboon.Core.Bases;
using Araboon.Core.MiddleWare;
using Araboon.Core.Translations;
using Araboon.Data.Helpers;
using Araboon.Data.Helpers.Resolvers.Mangas;
using Araboon.Infrastructure;
using Araboon.Infrastructure.Data;
using Araboon.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Araboon.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Initialize Encryption Key
            var encryptionSettings = new EncryptionSettings();
            builder.Configuration.GetSection(nameof(EncryptionSettings)).Bind(encryptionSettings);
            EncryptionHelper.Initialize(encryptionSettings.Key);
            #endregion
            #region SQL Srver Connection
            builder.Services.AddDbContext<AraboonDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AraboonConnection"))
                .UseLazyLoadingProxies();
            });
            #endregion
            #region Dependancy injection
            builder.Services.AddModuleInfrastructureServices(builder.Configuration)
                            .AddModuleServiceServices()
                            .AddModuleCoreServices();
            #endregion
            #region Localization
            builder.Services.AddControllersWithViews();
            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "";
            });
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar"),
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            #endregion
            #region AllowCORS
            var CORS = "_cors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: CORS,
                                  policy =>
                                  {
                                      policy.AllowAnyHeader();
                                      policy.AllowAnyMethod();
                                      policy.AllowAnyOrigin();
                                  });
            });
            #endregion
            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.CacheProfiles.Add("DefaultCache", new CacheProfile()
                {
                    Duration = 300,
                    Location = ResponseCacheLocation.Any,
                    NoStore = false,
                    VaryByHeader = "Accept-Language"
                });
                options.CacheProfiles.Add("PageNumberCache", new CacheProfile()
                {
                    Duration = 300,
                    Location = ResponseCacheLocation.Any,
                    NoStore = false,
                    VaryByHeader = "Accept-Language",
                    VaryByQueryKeys = new[] { "pageNumber" }
                });
                options.CacheProfiles.Add("ClientMangaCache", new CacheProfile()
                {
                    Duration = 300,
                    Location = ResponseCacheLocation.Client,
                    NoStore = false,
                    VaryByHeader = "Accept-Language,Authorization",
                    VaryByQueryKeys = new[] { "pageNumber" }
                });
            });
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var stringLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedTranslation>>();
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(
                                e => stringLocalizer is not null ? stringLocalizer[SharedTranslationKeys.ValidationError]
                                : e.ErrorMessage
                                ).ToArray()
                        );
                    return new BadRequestObjectResult(new ApiResponse()
                    {
                        Errors = errors,
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Succeeded = false,
                        Message = stringLocalizer[SharedTranslationKeys.ValidationFailed]
                    });
                };
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddTransient<MangaDateFormatResolver>();
            builder.Services.AddSingleton<IHostEnvironment>(sp => sp.GetRequiredService<IWebHostEnvironment>());
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });
            builder.Services.AddResponseCaching();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseResponseCaching();
            app.UseRequestLocalization(locOptions.Value);
            app.UseCors(CORS);
            app.UseMiddleware<ErrorHandlerMiddleWare>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            #region Localization Middleware
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            #endregion
            app.MapControllers();
            app.Run();
        }
    }
}
