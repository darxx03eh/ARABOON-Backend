using Araboon.Core.ResponseHelper;
using Araboon.Core.Translations;
using Araboon.Data.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;

namespace Araboon.Core.Middlewares
{
    public class LoginRateLimitMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private static readonly ConcurrentDictionary<string, (DateTime WindowStart, int FailCount)> attempts = new();

        public LoginRateLimitMiddleware(RequestDelegate next, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.next = next;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments($"/{Router.AuthenticationRouting.SignIn}", StringComparison.OrdinalIgnoreCase))
            {
                await next(context);
                return;
            }

            var username = context.Request.Headers["Rate-Limiting-Key"].ToString();
            if (string.IsNullOrWhiteSpace(username))
                username = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (attempts.TryGetValue(username, out var record))
            {
                if (record.WindowStart.AddMinutes(15) > DateTime.UtcNow && record.FailCount >= 5)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await ResponseHandler.WriteJsonResponse(
                        context,
                        System.Net.HttpStatusCode.TooManyRequests,
                        stringLocalizer[SharedTranslationKeys.YouHaveExceededTheLimitForSendingLoginRequestPleaseTryAgainLater]
                    );
                    return;
                }
                else if (record.WindowStart.AddMinutes(15) <= DateTime.UtcNow) attempts[username] = (DateTime.UtcNow, 0);
            }

            await next(context);

            bool isFailed = context.Response.StatusCode != StatusCodes.Status200OK;
            if (isFailed)
            {
                attempts.AddOrUpdate(
                    username,
                    (DateTime.UtcNow, 1),
                    (_, old) => (old.WindowStart, old.FailCount + 1)
                );
            }
        }
    }
}