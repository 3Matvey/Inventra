using Inventra.Application.Common.Interfaces;
using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Inventory> Inventories => Set<Inventory>();

    public DbSet<InventoryAccessGrant> InventoryAccessGrants => Set<InventoryAccessGrant>();

    public DbSet<InventoryComment> InventoryComments => Set<InventoryComment>();

    public DbSet<InventoryField> InventoryFields => Set<InventoryField>();

    public DbSet<InventoryIdFormatElement> InventoryIdFormatElements => Set<InventoryIdFormatElement>();

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    public DbSet<InventoryTag> InventoryTags => Set<InventoryTag>();

    public DbSet<ItemFieldValue> ItemFieldValues => Set<ItemFieldValue>();

    public DbSet<ItemLike> ItemLikes => Set<ItemLike>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
