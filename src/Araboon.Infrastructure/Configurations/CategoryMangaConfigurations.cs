using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CategoryMangaConfigurations : IEntityTypeConfiguration<CategoryManga>
    {
        public void Configure(EntityTypeBuilder<CategoryManga> builder)
        {
            builder.HasKey(x => new { x.CategoryID, x.MangaID });
            builder.HasOne(x => x.Manga)
                .WithMany(x => x.CategoryMangas)
                .HasForeignKey(x => x.MangaID);
            builder.HasOne(x => x.Category)
                .WithMany(x => x.CategoryMangas)
                .HasForeignKey(x => x.CategoryID);
        }
    }
}
