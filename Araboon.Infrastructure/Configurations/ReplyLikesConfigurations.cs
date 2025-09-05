using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ReplyLikesConfigurations : IEntityTypeConfiguration<ReplyLikes>
    {
        public void Configure(EntityTypeBuilder<ReplyLikes> builder)
        {
            builder.HasKey(x => new { x.UserId, x.ReplyId });
            builder.HasOne(x => x.User)
                .WithMany(x => x.ReplyLikes)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Reply)
                 .WithMany(x => x.ReplyLikes)
                 .HasForeignKey(x => x.ReplyId);
        }
    }
}
