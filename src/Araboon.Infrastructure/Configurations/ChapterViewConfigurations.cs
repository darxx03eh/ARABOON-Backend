using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ChapterViewConfigurations : IEntityTypeConfiguration<ChapterView>
    {
        public void Configure(EntityTypeBuilder<ChapterView> builder)
        {
            builder.HasKey(x => new { x.UserID, x.MangaID, x.ChapterID });
            builder.HasOne(x => x.User)
                .WithMany(x => x.ChapterViews)
                .HasForeignKey(x => x.UserID);
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.ChapterViews)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.Chapter)
                .WithMany(x => x.ChapterViews)
                .HasForeignKey(x => x.ChapterID);
        }
    }
}
