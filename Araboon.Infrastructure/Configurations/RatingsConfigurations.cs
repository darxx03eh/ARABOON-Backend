using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class RatingsConfigurations : IEntityTypeConfiguration<Ratings>
    {
        public void Configure(EntityTypeBuilder<Ratings> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.Ratings)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Ratings)
                .HasForeignKey(x => x.UserID);
        }
    }
}
