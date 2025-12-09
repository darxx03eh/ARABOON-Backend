using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ReadingLaterConfigurations : IEntityTypeConfiguration<ReadingLater>
    {
        public void Configure(EntityTypeBuilder<ReadingLater> builder)
        {
            builder.HasKey(x => new { x.UserID, x.MangaID });
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.ReadingLaters)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.User)
                .WithMany(x => x.ReadingLaters)
                .HasForeignKey(x => x.UserID);
        }
    }
}
