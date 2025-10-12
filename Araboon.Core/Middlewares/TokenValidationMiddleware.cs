using Araboon.Core.Bases;
using Araboon.Core.Translations;
using Araboon.Data.Helpers;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Araboon.Core.Middleware
{
    public class TokenValidationMiddleware
    {
        
        private readonly RequestDelegate next;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly JwtSettings jwtSettings;
        private readonly JsonSerializerOptions JSONOPTIONS = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly string[] PUBLICPATHS = new string[]
        {
            // authentication paths
            Router.AuthenticationRouting.SignIn,
            Router.AuthenticationRouting.RegistrationUser,
            Router.AuthenticationRouting.EmailConfirmation,
            Router.AuthenticationRouting.SendConfirmationEmail,
            Router.AuthenticationRouting.SendForgetPasswordEmail,
            Router.AuthenticationRouting.ForgetPasswordConfirmation,
            Router.AuthenticationRouting.ResetPassword,
            Router.AuthenticationRouting.ValidateAccessToken,
            Router.AuthenticationRouting.GenerateRefreshToken,

            // categories paths
            Router.CategoryRouting.GetCategories,

            // chapters paths
            Router.ChaptersRouting.ViewChaptersForSpecificMangaByLanguage,
            Router.ChaptersRouting.ViewChapterImages,

            // mangas paths
            Router.MangaRouting.GetCategoriesHomePageMangas,
            Router.MangaRouting.GetHottestMangas,
            Router.MangaRouting.GetMangaByCategoryName,
            Router.MangaRouting.GetPaginatedHottestManga,
            Router.MangaRouting.GetMangaByStatus,
            Router.MangaRouting.MangaSearch,
            "Api/V1/Manga/GetMangaByID",

            // users paths
            "Api/V1/users/profile",
            Router.UserRouting.ChangeEmailConfirmation,

            // comments paths 
            "Api/V1/comments"
        };

        public TokenValidationMiddleware(RequestDelegate next, IStringLocalizer<SharedTranslation> stringLocalizer, JwtSettings jwtSettings)
        {
            this.next = next;
            this.stringLocalizer = stringLocalizer;
            this.jwtSettings = jwtSettings;
        }
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value.Substring(1) ?? string.Empty;
            var header = context.Request.Headers["Authorization"].FirstOrDefault();
            string token = null;
            if (!string.IsNullOrEmpty(header) && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = header.Substring("Bearer ".Length).Trim();
            var isPublic = PUBLICPATHS.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(token))
            {
                if (!isPublic)
                {
                    await WriteJsonResponse(context, HttpStatusCode.Unauthorized, stringLocalizer[SharedTranslationKeys.Unauthorized]);
                    return;
                }
                await next(context);
                return;
            }
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    ValidateAudience = true,
                    ValidAudiences = new[] { jwtSettings.Audience },
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    ClockSkew = TimeSpan.Zero
                };
                var principal = handler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                if (validatedToken is not JwtSecurityToken jwtToken)
                {
                    await WriteJsonResponse(context, HttpStatusCode.Unauthorized, stringLocalizer[SharedTranslationKeys.InvalidTokenFormat]);
                    return;
                }
            }
            catch (SecurityTokenExpiredException exp)
            {
                await WriteJsonResponse(context, HttpStatusCode.Unauthorized, stringLocalizer[SharedTranslationKeys.TokenExpired]);
                return;
            }
            catch
            {
                await WriteJsonResponse(context, HttpStatusCode.Unauthorized, stringLocalizer[SharedTranslationKeys.InvalidToken]);
                return;
            }
            await next(context);
        }
        private async Task WriteJsonResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {

            if (context.Response.HasStarted) 
                return;

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new ApiResponse
            {
                Succeeded = false,
                StatusCode = statusCode,
                Message = message
            };

            var json = JsonSerializer.Serialize(response, JSONOPTIONS);
            await context.Response.WriteAsync(json, Encoding.UTF8);
        }
    }
}
