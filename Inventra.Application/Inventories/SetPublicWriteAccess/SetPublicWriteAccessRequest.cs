namespace Inventra.Application.Inventories.SetPublicWriteAccess;

public sealed record SetPublicWriteAccessRequest(
    Guid InventoryId,
    long ExpectedVersion,
    bool IsPublic);
