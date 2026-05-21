using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Application.Inventories.Exports;

public sealed class ExportInventoryUseCase(
    IInventoryExportQueries queries,
    IInventoryExportFileWriter writer)
    : IUseCase
{
    public async Task<Result<InventoryExportFileDto>> ExecuteAsync(
        ExportInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var formatResult = ParseFormat(request.Format);

        if (!formatResult.IsSuccess)
            return formatResult.Error;

        var export = await queries.GetAsync(request.InventoryId, cancellationToken);

        return export is null
            ? InventoryErrors.NotFound(request.InventoryId)
            : writer.CreateFile(export, formatResult.Value);
    }

    private static Result<InventoryExportFormat> ParseFormat(string format)
    {
        return format.Trim().ToLowerInvariant() switch
        {
            "csv" => InventoryExportFormat.Csv,
            "xlsx" => InventoryExportFormat.Xlsx,
            _ => InventoryExportErrors.UnsupportedFormat(format)
        };
    }
}
