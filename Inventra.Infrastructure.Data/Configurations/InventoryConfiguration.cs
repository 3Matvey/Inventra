using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.DescriptionMarkdown).HasColumnType("text");
        builder.Property(x => x.ImageUrl).HasMaxLength(2048);
        builder.Property(x => x.ImagePublicId).HasMaxLength(255);
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasIndex(x => new { x.Title, x.DescriptionMarkdown })
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("simple");

        builder.HasMany(x => x.Fields).WithOne().HasForeignKey(x => x.InventoryId);
        builder.HasMany(x => x.AccessGrants).WithOne().HasForeignKey(x => x.InventoryId);
        builder.HasMany(x => x.IdFormatElements).WithOne().HasForeignKey(x => x.InventoryId);
        builder.HasMany(x => x.Tags).WithOne().HasForeignKey(x => x.InventoryId);
        builder.HasMany(x => x.Comments).WithOne().HasForeignKey(x => x.InventoryId);

        builder.Navigation(x => x.Fields).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.AccessGrants).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.IdFormatElements).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Tags).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Comments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
