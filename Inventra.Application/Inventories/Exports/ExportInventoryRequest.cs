namespace Inventra.Application.Inventories.Exports;

public sealed record ExportInventoryRequest(
    Guid InventoryId,
    string Format);
