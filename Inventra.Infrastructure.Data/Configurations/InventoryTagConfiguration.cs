using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryTagConfiguration : IEntityTypeConfiguration<InventoryTag>
{
    public void Configure(EntityTypeBuilder<InventoryTag> builder)
    {
        builder.ToTable("inventory_tags");
        builder.ConfigureId();

        builder.HasIndex(x => new { x.InventoryId, x.TagId }).IsUnique();
    }
}
