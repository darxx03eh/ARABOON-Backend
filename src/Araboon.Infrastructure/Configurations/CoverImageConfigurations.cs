using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CoverImageConfigurations : IEntityTypeConfiguration<CoverImage>
    {
        public void Configure(EntityTypeBuilder<CoverImage> builder)
        {
            builder.HasOne(x => x.User)
                .WithOne(x => x.CoverImage)
                .HasForeignKey<CoverImage>(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
