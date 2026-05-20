using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventorySequenceConfiguration : IEntityTypeConfiguration<InventorySequence>
{
    public void Configure(EntityTypeBuilder<InventorySequence> builder)
    {
        builder.HasKey(x => x.InventoryId);
        builder.Property(x => x.NextValue).IsRequired();
        builder.HasOne<Inventory>().WithOne().HasForeignKey<InventorySequence>(x => x.InventoryId);
    }
}
