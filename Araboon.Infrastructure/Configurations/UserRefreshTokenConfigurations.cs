using Araboon.Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class UserRefreshTokenConfigurations : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID)
                .ValueGeneratedOnAdd();
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserRefreshTokens)
                .HasForeignKey(x => x.UserID);
        }
    }
}
