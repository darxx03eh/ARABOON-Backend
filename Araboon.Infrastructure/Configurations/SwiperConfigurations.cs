using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class SwiperConfigurations : IEntityTypeConfiguration<Swiper>
    {
        public void Configure(EntityTypeBuilder<Swiper> builder)
        {
            builder.Property(x => x.IsActive)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(x => x.UpdatedAt)
                .HasDefaultValue(DateTime.UtcNow);
        }
    }
}
