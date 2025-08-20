using Araboon.Data.Entities;
using Araboon.Data.Response.Mangas.Queries;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Araboon.Data.Helpers.Resolvers.Mangas
{
    public class MangaDateFormatResolver : IValueResolver<Manga, GetMangaByIDResponse, String>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public MangaDateFormatResolver(IHttpContextAccessor httpContextAccessor)
            => this.httpContextAccessor = httpContextAccessor;
        public String Resolve(Manga source, GetMangaByIDResponse destination, String destMember, ResolutionContext context)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();

            var lang = "en";
            if (!String.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            var culture = lang == "ar" ? new CultureInfo("ar") : new CultureInfo("en");
            return source.CreatedAt.ToString(
                culture.TwoLetterISOLanguageName == "ar" ? "dd MMMM yyyy" : "MMMM dd, yyyy",
                culture);
        }
    }
}
