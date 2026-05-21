namespace Inventra.Application.Inventories.Exports;

public static class InventoryExportErrors
{
    public static Error UnsupportedFormat(string format) =>
        Error.BadRequest(
            "InventoryExport.UnsupportedFormat",
            $"Inventory export format '{format}' is not supported.");
}
