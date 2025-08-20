using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class NotificationsConfigurations : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder.HasKey(x => new { x.UserID, x.MangaID });
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserID);
        }
    }
}
