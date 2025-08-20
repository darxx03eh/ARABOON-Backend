using Araboon.Data.Entities;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Araboon.Infrastructure.Resolvers.ChaptersResolver
{
    public class IsViewResolver : IValueResolver<Chapter, ChaptersResponse, Boolean>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AraboonDbContext _context;
        public IsViewResolver(IHttpContextAccessor httpContextAccessor, AraboonDbContext _context)
        {
            this.httpContextAccessor = httpContextAccessor;
            this._context = _context;
        }
        public Boolean Resolve(Chapter source, ChaptersResponse destination, Boolean destMember, ResolutionContext context)
        {
            try
            {
                var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return false;
                var token = authHeader.Substring("Bearer ".Length);
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
                if (String.IsNullOrEmpty(userId))
                    return false;
                return _context.ChapterViews.Any(c => c.UserID.ToString().Equals(userId)&&c.ChapterID.Equals(source.ChapterID)); 
            }
            catch
            {
                return false;
            }
        }
    }
}
