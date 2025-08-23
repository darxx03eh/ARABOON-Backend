using Araboon.Data.Entities;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Araboon.Infrastructure.Resolvers.MangasResolver
{
    public class IsCompletedReadingResolver : IValueResolver<Manga, GetMangaByIDResponse, bool>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AraboonDbContext _context;
        public IsCompletedReadingResolver(IHttpContextAccessor httpContextAccessor, AraboonDbContext _context)
        {
            this.httpContextAccessor = httpContextAccessor;
            this._context = _context;
        }
        public bool Resolve(Manga source, GetMangaByIDResponse destination, bool destMember, ResolutionContext context)
        {
            try
            {
                var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                    return false;
                var token = authHeader.Substring("Bearer ".Length);
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
                if (string.IsNullOrEmpty(userId))
                    return false;
                return _context.CompletedReads.Any(f => f.UserID.ToString().Equals(userId) && f.MangaID.Equals(source.MangaID));
            }
            catch
            {
                return false;
            }
        }
    }
}
