using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Data.Configurations;

internal class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ConfigureId();

        builder.Property(x => x.UserName).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();

        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
