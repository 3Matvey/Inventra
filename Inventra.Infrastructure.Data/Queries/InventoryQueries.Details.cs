using Inventra.Application.Inventories.Queries.Dto;

namespace Inventra.Infrastructure.Data.Queries;

internal partial class InventoryQueries
{
    public async Task<InventoryDetailsDto?> GetDetailsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetInventoryDetailsBaseAsync(inventoryId, cancellationToken);

        return inventory is null
            ? null
            : await FillDetailsAsync(inventory, inventoryId, cancellationToken);
    }

    private async Task<InventoryDetailsDto> FillDetailsAsync(
        InventoryDetailsDto inventory,
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return inventory with
        {
            Tags = await GetTagsAsync(inventoryId, cancellationToken),
            Fields = await GetFieldsAsync(inventoryId, cancellationToken),
            IdFormatElements = await GetIdFormatElementsAsync(inventoryId, cancellationToken),
            AccessUsers = await GetAccessUsersAsync(inventoryId, cancellationToken)
        };
    }

    private async Task<InventoryDetailsDto?> GetInventoryDetailsBaseAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await InventoryDetailsBaseQuery(inventoryId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<InventoryDetailsDto> InventoryDetailsBaseQuery(Guid inventoryId)
    {
        return
            from inventory in dbContext.Inventories.AsNoTracking()
            join category in dbContext.Categories.AsNoTracking() on inventory.CategoryId equals category.Id
            join owner in dbContext.UserAccounts.AsNoTracking() on inventory.OwnerId equals owner.Id
            where inventory.Id == inventoryId
            select ToDetailsBase(inventory, category, owner);
    }

    private static InventoryDetailsDto ToDetailsBase(
        Inventory inventory,
        Category category,
        UserAccount owner)
    {
        return new InventoryDetailsDto(
            inventory.Id,
            inventory.Title,
            inventory.DescriptionMarkdown,
            inventory.ImageUrl,
            new CategoryDto(category.Id, category.Name),
            new UserSummaryDto(owner.Id, owner.UserName),
            inventory.IsPublicWriteAccess,
            inventory.Version,
            Array.Empty<TagDto>(),
            Array.Empty<InventoryFieldDefinitionDto>(),
            Array.Empty<InventoryIdFormatElementDto>(),
            Array.Empty<InventoryAccessUserDto>());
    }

    private async Task<IReadOnlyCollection<InventoryFieldDefinitionDto>> GetFieldsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await dbContext.InventoryFields
            .AsNoTracking()
            .Where(x => x.InventoryId == inventoryId)
            .OrderBy(x => x.Order)
            .Select(x => ToFieldDefinition(x))
            .ToArrayAsync(cancellationToken);
    }

    private static InventoryFieldDefinitionDto ToFieldDefinition(InventoryField field)
    {
        return new InventoryFieldDefinitionDto(
            field.Id,
            field.Type,
            field.Title,
            field.Description,
            field.ShowInTable,
            field.Order);
    }

    private async Task<IReadOnlyCollection<InventoryIdFormatElementDto>> GetIdFormatElementsAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await dbContext.InventoryIdFormatElements
            .AsNoTracking()
            .Where(x => x.InventoryId == inventoryId)
            .OrderBy(x => x.Order)
            .Select(x => ToIdFormatElement(x))
            .ToArrayAsync(cancellationToken);
    }

    private static InventoryIdFormatElementDto ToIdFormatElement(InventoryIdFormatElement element)
    {
        return new InventoryIdFormatElementDto(
            element.Id,
            element.Type,
            element.Value,
            element.Format,
            element.Order);
    }

    private async Task<IReadOnlyCollection<InventoryAccessUserDto>> GetAccessUsersAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        return await AccessUsersQuery(inventoryId)
            .ToArrayAsync(cancellationToken);
    }

    private IQueryable<InventoryAccessUserDto> AccessUsersQuery(Guid inventoryId)
    {
        return
            from grant in dbContext.InventoryAccessGrants.AsNoTracking()
            join user in dbContext.UserAccounts.AsNoTracking() on grant.UserId equals user.Id
            where grant.InventoryId == inventoryId
            orderby user.UserName
            select ToAccessUser(grant, user);
    }

    private static InventoryAccessUserDto ToAccessUser(
        InventoryAccessGrant grant,
        UserAccount user)
    {
        return new InventoryAccessUserDto(
            user.Id,
            user.UserName,
            user.Email,
            grant.GrantedAt);
    }
}
