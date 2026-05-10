using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class ItemLikeConfiguration : IEntityTypeConfiguration<ItemLike>
{
    public void Configure(EntityTypeBuilder<ItemLike> builder)
    {
        builder.ToTable("item_likes");
        builder.ConfigureId();

        builder.HasIndex(x => new { x.ItemId, x.UserId }).IsUnique();
    }
}
