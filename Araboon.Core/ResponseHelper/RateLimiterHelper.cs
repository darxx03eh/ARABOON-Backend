using Araboon.Core.Translations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Araboon.Core.ResponseHelper
{
    public static class RateLimiterHelper
    {
        public static async Task HandleRejectedAsync(OnRejectedContext context, string translationKey)
        {
            var httpContext = context.HttpContext;
            var stringLocalizer = httpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedTranslation>>();

            var culture = httpContext.Request.Headers["Accept-Language"].ToString();
            if (!string.IsNullOrEmpty(culture))
            {
                try
                {
                    var ci = new CultureInfo(culture);
                    CultureInfo.CurrentCulture = ci;
                    CultureInfo.CurrentUICulture = ci;
                }
                catch {  }
            }

            await ResponseHandler.WriteJsonResponse(
                httpContext,
                System.Net.HttpStatusCode.TooManyRequests,
                stringLocalizer[translationKey]
            );
        }
    }
}
