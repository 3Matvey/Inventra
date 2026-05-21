using ClosedXML.Excel;
using CsvHelper;
using Inventra.Application.Inventories.Exports;
using Inventra.Application.Inventories.Exports.Dto;
using Inventra.Domain.Enums;
using System.Globalization;

namespace Inventra.Infrastructure.Export;

internal sealed class InventoryExportFileWriter : IInventoryExportFileWriter
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
                WriteXlsx(export, output);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    private static async Task WriteCsvAsync(
        InventoryExportDto export,
        Stream output,
        CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(output, leaveOpen: true);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        WriteCsvHeader(csv, export.Fields);
        WriteCsvRows(csv, export);
        await writer.FlushAsync(cancellationToken);
    }

    private static void WriteCsvHeader(
        CsvWriter csv,
        IEnumerable<InventoryExportFieldDto> fields)
    {
        foreach (var header in FixedHeaders().Concat(fields.Select(x => x.Title)))
            csv.WriteField(header);

        csv.NextRecord();
    }

    private static void WriteCsvRows(CsvWriter csv, InventoryExportDto export)
    {
        foreach (var item in export.Items)
            WriteCsvRow(csv, export.Fields, item);
    }

    private static void WriteCsvRow(
        CsvWriter csv,
        IReadOnlyCollection<InventoryExportFieldDto> fields,
        InventoryExportItemDto item)
    {
        foreach (var value in FixedValues(item))
            csv.WriteField(value);

        foreach (var field in fields)
            csv.WriteField(FormatFieldValue(field, item.Values));

        csv.NextRecord();
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

    private static string[] FixedHeaders()
        => ["Custom ID", "Created By", "Created At", "Updated At", "Likes"];

    private static string[] FixedValues(InventoryExportItemDto item)
    {
        return
        [
            item.CustomId,
            item.CreatedByUserName,
            item.CreatedAt.ToString("O", CultureInfo.InvariantCulture),
            item.UpdatedAt?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty,
            item.LikesCount.ToString(CultureInfo.InvariantCulture)
        ];
    }

    private static string FormatFieldValue(
        InventoryExportFieldDto field,
        IReadOnlyCollection<InventoryExportValueDto> values)
    {
        var value = values.FirstOrDefault(x => x.FieldId == field.FieldId);

        return value is null ? string.Empty : FormatFieldValue(field.Type, value);
    }

    private static string FormatFieldValue(
        InventoryFieldType type,
        InventoryExportValueDto value)
    {
        return type switch
        {
            InventoryFieldType.Number => value.NumberValue?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            InventoryFieldType.Boolean => value.BooleanValue?.ToString() ?? string.Empty,
            _ => value.TextValue ?? string.Empty
        };
    }

    private static InventoryExportFileDto ToFile(
        InventoryExportDto export,
        InventoryExportFormat format,
        string extension,
        string contentType)
    {
        return new InventoryExportFileDto(
            $"{Slugify(export.InventoryTitle)}.{extension}",
            contentType,
            format,
            export);
    }

    private static string Slugify(string value)
    {
        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Select(x => char.IsLetterOrDigit(x) ? x : '-')
            .ToArray();

        var slug = new string(chars).Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "inventory-export" : slug;
    }
}
