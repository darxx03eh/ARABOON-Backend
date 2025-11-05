using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Resolvers.ChaptersResolver
{
    public class ChapterIsArabicResolver : IValueResolver<Chapter, ChaptersResponse, bool>
    {
        private readonly AraboonDbContext _context;
        public ChapterIsArabicResolver(AraboonDbContext _context)
            => this._context = _context;
        public bool Resolve(Chapter source, ChaptersResponse destination, bool destMember, ResolutionContext context)
            => _context.Chapters.Any(
                chapter => EF.Functions.Like(chapter.Language, "arabic")
                && chapter.ChapterID.Equals(source.ChapterID)
            );
    }
}
