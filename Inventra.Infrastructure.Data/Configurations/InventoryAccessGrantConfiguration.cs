using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryAccessGrantConfiguration : IEntityTypeConfiguration<InventoryAccessGrant>
{
    public void Configure(EntityTypeBuilder<InventoryAccessGrant> builder)
    {
        builder.ToTable("inventory_access_grants");
        builder.ConfigureId();

        builder.HasIndex(x => new { x.InventoryId, x.UserId }).IsUnique();
    }
}
