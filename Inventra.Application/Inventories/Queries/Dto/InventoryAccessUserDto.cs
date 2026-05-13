namespace Inventra.Application.Inventories.Queries.Dto;

/// <summary>
/// Represents a user explicitly granted write access to an inventory.
/// </summary>
public sealed record InventoryAccessUserDto(
    Guid UserId,
    string UserName,
    string Email,
    DateTimeOffset GrantedAt);
