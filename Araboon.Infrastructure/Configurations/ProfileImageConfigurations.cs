using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ProfileImageConfigurations : IEntityTypeConfiguration<ProfileImage>
    {
        public void Configure(EntityTypeBuilder<ProfileImage> builder)
        {
            builder.HasOne(x => x.User)
                .WithOne(x => x.ProfileImage)
                .HasForeignKey<ProfileImage>(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.X)
                .HasDefaultValue(0.0);
            builder.Property(x => x.Y)
                .HasDefaultValue(0.0);
            builder.Property(x => x.Scale)
                .HasDefaultValue(1.2);
            builder.Property(x => x.Rotate)
                .HasDefaultValue(0.0);
        }
    }
}
