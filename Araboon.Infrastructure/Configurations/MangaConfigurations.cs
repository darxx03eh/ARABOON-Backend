using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class MangaConfigurations : IEntityTypeConfiguration<Manga>
    {
        public void Configure(EntityTypeBuilder<Manga> builder)
        {
            builder.HasKey(x => x.MangaID);
            builder.Property(x => x.MangaID)
                .ValueGeneratedOnAdd();

            builder.HasMany(x => x.Favorites)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CompletedReads)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CurrentlyReadings)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ReadingLaters)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CategoryMangas)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Chapters)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Comments)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ChapterViews)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Ratings)
                .WithOne(x => x.Manga)
                .HasForeignKey(x => x.MangaID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.StatusEn)
                .HasDefaultValue("ongoing");
            builder.Property(x => x.StatusAr)
                .HasDefaultValue("مستمرة");
            builder.Property(x => x.TypeEn)
                .HasDefaultValue("manga");
            builder.Property(x => x.TypeAr)
                .HasDefaultValue("مانجا");
            builder.Property(x => x.Rate)
                .HasDefaultValue(0.0);

            builder.HasCheckConstraint("CK_Manga_Rate", "[Rate] >= 0 And [Rate] <= 5");
            builder.Property(x => x.RatingsCount)
                .HasDefaultValue(0);

            builder.HasIndex(x => x.MangaNameEn).IsUnique();
            builder.HasIndex(x => x.MangaNameAr).IsUnique();
        }
    }
}
