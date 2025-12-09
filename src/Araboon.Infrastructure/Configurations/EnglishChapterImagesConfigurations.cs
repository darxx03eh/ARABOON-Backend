using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class EnglishChapterImagesConfigurations : IEntityTypeConfiguration<EnglishChapterImages>
    {
        public void Configure(EntityTypeBuilder<EnglishChapterImages> builder)
        {
            builder.HasKey(x => x.ImageID);
            builder.Property(x => x.ImageID)
                .ValueGeneratedOnAdd();
            builder.HasOne(x => x.Chapter)
                .WithMany(x => x.EnglishChapterImages)
                .HasForeignKey(x => x.ChapterID);
        }
    }
}
