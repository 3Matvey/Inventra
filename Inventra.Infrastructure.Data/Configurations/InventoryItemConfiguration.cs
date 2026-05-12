using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.CustomId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Version).IsConcurrencyToken();

        builder.HasIndex(x => new { x.InventoryId, x.CustomId }).IsUnique();
        builder.HasIndex(x => new { x.InventoryId, x.SequenceNumber })
            .IsUnique()
            .HasFilter("sequence_number IS NOT NULL");

        builder.HasMany(x => x.FieldValues).WithOne().HasForeignKey(x => x.ItemId);
        builder.HasMany(x => x.Likes).WithOne().HasForeignKey(x => x.ItemId);

        builder.Navigation(x => x.FieldValues).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Likes).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
