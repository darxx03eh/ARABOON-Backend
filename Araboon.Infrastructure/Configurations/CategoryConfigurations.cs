using Araboon.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Araboon.Infrastructure.Configurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.CategoryID);
            builder.Property(x => x.CategoryID)
                .ValueGeneratedOnAdd();
            builder.HasMany(x => x.CategoryMangas)
                .WithOne(x => x.Category)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
