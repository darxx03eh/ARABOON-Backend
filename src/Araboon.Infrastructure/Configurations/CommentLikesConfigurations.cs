using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CommentLikesConfigurations : IEntityTypeConfiguration<CommentLikes>
    {
        public void Configure(EntityTypeBuilder<CommentLikes> builder)
        {
            builder.HasKey(x => new { x.UserId, x.CommentId });
            builder.HasOne(x => x.User)
                .WithMany(x => x.CommentLikes)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Comment)
                .WithMany(x => x.CommentLikes)
                .HasForeignKey(x => x.CommentId);
        }
    }
}
