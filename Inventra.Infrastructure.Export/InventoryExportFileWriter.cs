using Inventra.Application.Inventories.Exports;
using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Infrastructure.Export;

internal sealed partial class InventoryExportFileWriter : IInventoryExportFileWriter
{
    private const string CsvContentType = "text/csv; charset=utf-8";
    private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public InventoryExportFileDto CreateFile(
        InventoryExportDto export,
        InventoryExportFormat format)
    {
        return format switch
        {
            InventoryExportFormat.Csv => ToFile(export, format, "csv", CsvContentType),
            InventoryExportFormat.Xlsx => ToFile(export, format, "xlsx", XlsxContentType),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    public async Task WriteAsync(
        InventoryExportDto export,
        InventoryExportFormat format,
        Stream output,
        CancellationToken cancellationToken = default)
    {
        switch (format)
        {
            case InventoryExportFormat.Csv:
                await WriteCsvAsync(export, output, cancellationToken);
                break;
            case InventoryExportFormat.Xlsx:
                await WriteXlsxAsync(export, output, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}
