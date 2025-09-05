using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ReplyConfigurations : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.HasKey(x => x.ReplyID);
            builder.Property(x => x.ReplyID)
                .ValueGeneratedOnAdd();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.UserID);

            builder.HasOne(x => x.Comment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.CommentID);

            builder.Property(x => x.Likes)
                .HasDefaultValue(0);

            builder.HasMany(x => x.ReplyLikes)
                .WithOne(x => x.Reply)
                .HasForeignKey(x => x.ReplyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
