using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Data.Response.Mangas.Queries;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Araboon.Data.Helpers.Resolvers.ChaptersResolver
{
    public class ChapterDateFormatResolver : IValueResolver<Chapter, ChaptersResponse, string>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public ChapterDateFormatResolver(IHttpContextAccessor httpContextAccessor)
            => this.httpContextAccessor = httpContextAccessor;
        public string Resolve(Chapter source, ChaptersResponse destination, string destMember, ResolutionContext context)
        {
            string lang = "en";
            if (context.Items.TryGetValue("lang", out var langObj) && langObj is string langStr)
                lang = langStr;
            var culture = lang == "ar" ? new CultureInfo("ar") : new CultureInfo("en");
            return source.CreatedAt.ToString(
                culture.TwoLetterISOLanguageName == "ar" ? "dd MMMM yyyy" : "MMMM dd, yyyy",
                culture
            );
        }
    }
}
