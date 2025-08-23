using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Araboon.Infrastructure.Resolvers.ChaptersResolver
{
    public class ChapterIsArabicResolver : IValueResolver<Chapter, ChaptersResponse, bool>
    {
        private readonly AraboonDbContext _context;
        public ChapterIsArabicResolver(AraboonDbContext _context)
            => this._context = _context;
        public bool Resolve(Chapter source, ChaptersResponse destination, bool destMember, ResolutionContext context)
            => _context.Chapters.Any(chapter => chapter.Language.ToLower().Equals("arabic"));
    }
}
