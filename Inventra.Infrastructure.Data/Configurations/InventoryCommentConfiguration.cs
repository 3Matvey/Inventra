using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class InventoryCommentConfiguration : IEntityTypeConfiguration<InventoryComment>
{
    public void Configure(EntityTypeBuilder<InventoryComment> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.BodyMarkdown).HasColumnType("text").IsRequired();
        builder.HasIndex(x => new { x.InventoryId, x.CreatedAt });
    }
}
