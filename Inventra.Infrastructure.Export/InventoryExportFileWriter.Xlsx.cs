using ClosedXML.Excel;
using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Infrastructure.Export;

internal sealed partial class InventoryExportFileWriter
{
    private static async Task WriteXlsxAsync(
        InventoryExportDto export,
        Stream output,
        CancellationToken cancellationToken)
    {
        using var buffer = new MemoryStream();

        WriteXlsx(export, buffer);
        buffer.Position = 0;

        await buffer.CopyToAsync(output, cancellationToken);
    }

    private static void WriteXlsx(InventoryExportDto export, Stream output)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Items");

        WriteXlsxHeader(worksheet, export.Fields);
        WriteXlsxRows(worksheet, export);
        worksheet.Columns().AdjustToContents();

        workbook.SaveAs(output);
    }

    private static void WriteXlsxHeader(
        IXLWorksheet worksheet,
        IReadOnlyCollection<InventoryExportFieldDto> fields)
    {
        var headers = FixedHeaders().Concat(fields.Select(x => x.Title)).ToArray();

        for (var index = 0; index < headers.Length; index++)
            worksheet.Cell(1, index + 1).Value = headers[index];

        worksheet.Row(1).Style.Font.Bold = true;
    }

    private static void WriteXlsxRows(IXLWorksheet worksheet, InventoryExportDto export)
    {
        var row = 2;

        foreach (var item in export.Items)
            WriteXlsxRow(worksheet, export.Fields, item, row++);
    }

    private static void WriteXlsxRow(
        IXLWorksheet worksheet,
        IReadOnlyCollection<InventoryExportFieldDto> fields,
        InventoryExportItemDto item,
        int row)
    {
        var values = FixedValues(item)
            .Concat(fields.Select(field => FormatFieldValue(field, item.Values)))
            .ToArray();

        for (var index = 0; index < values.Length; index++)
            worksheet.Cell(row, index + 1).Value = values[index];
    }
}
