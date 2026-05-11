using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class ItemFieldValueConfiguration : IEntityTypeConfiguration<ItemFieldValue>
{
    public void Configure(EntityTypeBuilder<ItemFieldValue> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.TextValue).HasColumnType("text");
        builder.Property(x => x.NumberValue).HasPrecision(18, 2);

        builder.HasIndex(x => new { x.ItemId, x.FieldId }).IsUnique();
    }
}
