using Araboon.Core;
using Araboon.Core.Bases;
using Araboon.Core.Middleware;
using Araboon.Core.Middlewares;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Infrastructure;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.Seeder;
using Araboon.Service;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
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
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Initialize Encryption Key
            var encryptionSettings = new EncryptionSettings();
            builder.Configuration.GetSection(nameof(EncryptionSettings)).Bind(encryptionSettings);
            var hangfireSettings = new HangfireSettings();
            builder.Configuration.GetSection(nameof(HangfireSettings)).Bind(hangfireSettings);
            EncryptionHelper.Initialize(encryptionSettings.Key);
            #endregion
            #region SQL Srver Connection
            builder.Services.AddDbContext<AraboonDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AraboonConnection"))
                .UseLazyLoadingProxies();
            });
            #region Hangfire
            builder.Services.AddHangfire(options =>
            {
                options.UseSqlServerStorage(builder.Configuration.GetConnectionString("AraboonConnection"));
            });
            builder.Services.AddHangfireServer();
            #endregion
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
                                      policy.WithOrigins("https://araboon.vercel.app")
                                            .AllowAnyMethod()
                                            .AllowAnyHeader()
                                            .AllowCredentials()
                                            .WithExposedHeaders("Content-Type", "Authorization", "Content-Length");
                                  });
            });
            #endregion

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
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
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "enter JWT token like this: Bearer {your token}"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            builder.Services.AddSingleton<IHostEnvironment>(sp => sp.GetRequiredService<IWebHostEnvironment>());
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(1);
            });
            builder.Services.AddResponseCaching();
            var app = builder.Build();
            #region Localization Middleware
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            #endregion
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect = true, // нои HTTPS
                        RequireSsl = true,
                        LoginCaseSensitive = true,
                        Users = new[]
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = hangfireSettings.UserName,
                                PasswordClear = hangfireSettings.Password
                            }
                        }
                    })
                }
            });
            app.Map("/", () => "Hangfire running");
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AraboonRole>>();
                await RoleSeeder.SeedAsync(roleManager);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseResponseCaching();
            app.UseCors(CORS);
            app.UseRateLimiter();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<TokenValidationMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
