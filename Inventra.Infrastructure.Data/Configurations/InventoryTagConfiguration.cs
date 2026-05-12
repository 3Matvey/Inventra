using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryTagConfiguration : IEntityTypeConfiguration<InventoryTag>
{
    public void Configure(EntityTypeBuilder<InventoryTag> builder)
    {
        builder.ConfigureId();

        builder.HasIndex(x => new { x.InventoryId, x.TagId }).IsUnique();
    }
}
