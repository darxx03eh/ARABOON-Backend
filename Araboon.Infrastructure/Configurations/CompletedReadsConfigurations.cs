using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CompletedReadsConfigurations : IEntityTypeConfiguration<CompletedReads>
    {
        public void Configure(EntityTypeBuilder<CompletedReads> builder)
        {
            builder.HasKey(x => new { x.UserID, x.MangaID });
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.CompletedReads)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.User)
                .WithMany(x => x.CompletedReads)
                .HasForeignKey(x => x.UserID);
        }
    }
}
