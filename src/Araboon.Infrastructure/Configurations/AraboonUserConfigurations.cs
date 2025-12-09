using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class AraboonUserConfigurations : IEntityTypeConfiguration<AraboonUser>
    {
        public void Configure(EntityTypeBuilder<AraboonUser> builder)
        {
            builder.HasMany(x => x.Favorites)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CompletedReads)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CurrentlyReadings)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ReadingLaters)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.UserRefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Comments)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Replies)
                .WithOne(x => x.FromUser)
                .HasForeignKey(x => x.FromUserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ToReplies)
                .WithOne(x => x.ToUser)
                .HasForeignKey(x => x.ToUserID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.ChapterViews)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CoverImage)
                .WithOne(x => x.User)
                .HasForeignKey<CoverImage>(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ProfileImage)
                .WithOne(x => x.User)
                .HasForeignKey<ProfileImage>(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Ratings)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CommentLikes)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ReplyLikes)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete (DeleteBehavior.Cascade);
        }
    }
}
