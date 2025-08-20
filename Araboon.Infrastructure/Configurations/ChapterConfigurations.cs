using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ChapterConfigurations : IEntityTypeConfiguration<Chapter>
    {
        public void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.HasKey(x => x.ChapterID);
            builder.Property(x => x.ChapterID)
                .ValueGeneratedOnAdd();
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.Chapters)
                .HasForeignKey(x => x.MangaID);
            builder.HasMany(x => x.ArabicChapterImages)
                .WithOne(x => x.Chapter)
                .HasForeignKey(x => x.ChapterID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.EnglishChapterImages)
                .WithOne(x => x.Chapter)
                .HasForeignKey(x => x.ChapterID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.ChapterViews)
                .WithOne(x => x.Chapter)
                .HasForeignKey(x => x.ChapterID)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.ReadersCount)
                .HasDefaultValue(0);
            builder.Property(x => x.Language)
                .HasDefaultValue("Arabic");
        }
    }
}
