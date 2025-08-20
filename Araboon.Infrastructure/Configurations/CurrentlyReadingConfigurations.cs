using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CurrentlyReadingConfigurations : IEntityTypeConfiguration<CurrentlyReading>
    {
        public void Configure(EntityTypeBuilder<CurrentlyReading> builder)
        {
            builder.HasKey(x => new { x.UserID, x.MangaID });
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.CurrentlyReadings)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.User)
                .WithMany(x => x.CurrentlyReadings)
                .HasForeignKey(x => x.UserID);
        }
    }
}
