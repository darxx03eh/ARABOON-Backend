using Araboon.Data.Entities;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Araboon.Data.Helpers.Resolvers.MangasResolver
{
    public class IsFavoriteResolver : IValueResolver<Manga, GetMangaByIDResponse, Boolean>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AraboonDbContext _context;
        public IsFavoriteResolver(IHttpContextAccessor httpContextAccessor, AraboonDbContext _context)
        {
            this.httpContextAccessor = httpContextAccessor;
            this._context = _context;
        }
        public Boolean Resolve(Manga source, GetMangaByIDResponse destination, Boolean destMember, ResolutionContext context)
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
                return _context.Favorites.Any(f => f.UserID.ToString().Equals(userId) && f.MangaID.Equals(source.MangaID));
            }
            catch
            {
                return false;
            }
        }
    }
}
