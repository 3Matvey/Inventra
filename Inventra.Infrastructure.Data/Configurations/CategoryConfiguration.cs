using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.ConfigureId();

        builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
