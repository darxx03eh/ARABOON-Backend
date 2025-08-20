using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class ArabicChapterImagesConfigurations : IEntityTypeConfiguration<ArabicChapterImages>
    {
        public void Configure(EntityTypeBuilder<ArabicChapterImages> builder)
        {
            builder.HasKey(x => x.ImageID);
            builder.Property(x => x.ImageID)
                .ValueGeneratedOnAdd();
            builder.HasOne(x => x.Chapter)
                .WithMany(x => x.ArabicChapterImages)
                .HasForeignKey(x => x.ChapterID);
        }
    }
}
