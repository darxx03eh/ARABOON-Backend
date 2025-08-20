using System.Reflection;
using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Data
{
    public class AraboonDbContext
        : IdentityDbContext<AraboonUser, AraboonRole, Int32,
          IdentityUserClaim<Int32>, IdentityUserRole<Int32>, IdentityUserLogin<Int32>, IdentityRoleClaim<Int32>, IdentityUserToken<Int32>>
    {
        public AraboonDbContext(DbContextOptions<AraboonDbContext> options)
            : base(options) { }
        public DbSet<AraboonUser> Users { get; set; }
        public DbSet<AraboonRole> Roles { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryManga> CategoryMangas { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ArabicChapterImages> ArabicChapterImages { get; set; }
        public DbSet<EnglishChapterImages> EnglishChapterImages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CompletedReads> CompletedReads { get; set; }
        public DbSet<CurrentlyReading> CurrentlyReadings { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<ReadingLater> ReadingLaters { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<ChapterView> ChapterViews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
