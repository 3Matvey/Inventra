using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryFieldConfiguration : IEntityTypeConfiguration<InventoryField>
{
    public void Configure(EntityTypeBuilder<InventoryField> builder)
    {
        builder.ToTable("inventory_fields");
        builder.ConfigureId();

        builder.Property(x => x.Title).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(512);

        builder.HasIndex(x => new { x.InventoryId, x.Order }).IsUnique();
    }
}
