using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryIdFormatElementConfiguration : IEntityTypeConfiguration<InventoryIdFormatElement>
{
    public void Configure(EntityTypeBuilder<InventoryIdFormatElement> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.Value).HasMaxLength(256);
        builder.Property(x => x.Format).HasMaxLength(64);

        builder.HasIndex(x => new { x.InventoryId, x.Order }).IsUnique();
    }
}
