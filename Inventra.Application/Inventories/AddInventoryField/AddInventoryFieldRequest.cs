using Inventra.Domain.Enums;

namespace Inventra.Application.Inventories.AddInventoryField;

public sealed record AddInventoryFieldRequest(
    Guid InventoryId,
    long ExpectedVersion,
    InventoryFieldType Type,
    string Title,
    string? Description,
    bool ShowInTable);
