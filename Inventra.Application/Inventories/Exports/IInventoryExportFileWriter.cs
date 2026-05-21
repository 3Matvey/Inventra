using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Application.Inventories.Exports;

/// <summary>
/// Writes inventory export data into a downloadable file format.
/// </summary>
public interface IInventoryExportFileWriter
{
    InventoryExportFileDto CreateFile(
        InventoryExportDto export,
        InventoryExportFormat format);

    Task WriteAsync(
        InventoryExportDto export,
        InventoryExportFormat format,
        Stream output,
        CancellationToken cancellationToken = default);
}
