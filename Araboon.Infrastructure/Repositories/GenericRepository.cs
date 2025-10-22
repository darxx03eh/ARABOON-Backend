using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.IdentityModel.Tokens.Jwt;

namespace Araboon.Infrastructure.Repositories
{
    public class GenericRepository<T>(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager) : IGenericRepository<T>
        where T : class
    {
        private readonly AraboonDbContext context = context;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager = userManager;

        public virtual async Task<T> GetByIdAsync(int id)
            => await context.Set<T>().FindAsync(id);
        public IQueryable<T> GetTableNoTracking()
            => context.Set<T>().AsNoTracking().AsQueryable();
        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await context.Set<T>().AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }
        public virtual async Task<T> AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task UpdateAsync(T entity)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }
        public virtual async Task DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
                context.Entry(entity).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync()
            => await context.SaveChangesAsync();
        public IDbContextTransaction BeginTransaction()
            => context.Database.BeginTransaction();
        public void Commit()
            => context.Database.CommitTransaction();
        public void RollBack() => context.Database.RollbackTransaction();
        public IQueryable<T> GetTableAsTracking()
            => context.Set<T>().AsQueryable();
        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
            await context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
            => await context.Database.BeginTransactionAsync();

        public async Task CommitAsync()
            => await context.Database.CommitTransactionAsync();
        public async Task RollBackAsync()
            => await context.Database.RollbackTransactionAsync();
        public string? ExtractUserIdFromToken()
        {
            var authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;
            var token = authHeader.Substring("Bearer ".Length);
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(token => token.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
            }
            catch
            {
                return null;
            }
        }
        public bool IsArabic()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();

            var lang = "en";
            if (!string.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            return lang.Equals("ar");
        }
        public async Task<bool> IsAdmin()
        {
            var userId = ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return false;

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return false;

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
                if (role.ToLower().Equals("admin"))
                    return true;
            return false;
        }
    }
}
