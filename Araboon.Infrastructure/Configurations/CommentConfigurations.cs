using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CommentConfigurations : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.CommentID);
            builder.Property(x => x.CommentID)
                .ValueGeneratedOnAdd();
            builder.HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserID);
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.MangaID);
            builder.HasMany(x => x.Replies)
                .WithOne(x => x.Comment)
                .HasForeignKey(x => x.CommentID)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.Likes)
                .HasDefaultValue(0);
            builder.HasMany(x => x.CommentLikes)
                .WithOne(x => x.Comment)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
