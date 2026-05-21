using Inventra.Application.Inventories.Exports.Dto;

namespace Inventra.Infrastructure.Export;

internal sealed partial class InventoryExportFileWriter
{
    private static async Task WriteCsvAsync(
        InventoryExportDto export,
        Stream output,
        CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(output, leaveOpen: true);

        await WriteCsvHeaderAsync(writer, export.Fields, cancellationToken);
        await WriteCsvRowsAsync(writer, export, cancellationToken);
        await writer.FlushAsync(cancellationToken);
    }

    private static async Task WriteCsvHeaderAsync(
        TextWriter writer,
        IEnumerable<InventoryExportFieldDto> fields,
        CancellationToken cancellationToken)
    {
        var headers = FixedHeaders().Concat(fields.Select(x => x.Title));

        await WriteCsvRecordAsync(writer, headers, cancellationToken);
    }

    private static async Task WriteCsvRowsAsync(
        TextWriter writer,
        InventoryExportDto export,
        CancellationToken cancellationToken)
    {
        foreach (var item in export.Items)
            await WriteCsvRowAsync(writer, export.Fields, item, cancellationToken);
    }

    private static async Task WriteCsvRowAsync(
        TextWriter writer,
        IReadOnlyCollection<InventoryExportFieldDto> fields,
        InventoryExportItemDto item,
        CancellationToken cancellationToken)
    {
        var values = FixedValues(item)
            .Concat(fields.Select(field => FormatFieldValue(field, item.Values)));

        await WriteCsvRecordAsync(writer, values, cancellationToken);
    }

    private static async Task WriteCsvRecordAsync(
        TextWriter writer,
        IEnumerable<string> values,
        CancellationToken cancellationToken)
    {
        bool isFirst = true;

        foreach (var value in values)
            isFirst = await WriteCsvValueAsync(writer, value, isFirst, cancellationToken);

        await writer.WriteAsync(Environment.NewLine.AsMemory(), cancellationToken);
    }

    private static async Task<bool> WriteCsvValueAsync(
        TextWriter writer,
        string value,
        bool isFirst,
        CancellationToken cancellationToken)
    {
        if (!isFirst)
            await writer.WriteAsync(",".AsMemory(), cancellationToken);

        await writer.WriteAsync(EscapeCsv(value).AsMemory(), cancellationToken);

        return false;
    }

    private static string EscapeCsv(string value)
    {
        return RequiresEscaping(value)
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }

    private static bool RequiresEscaping(string value)
    {
        return value.Contains(',')
               || value.Contains('"')
               || value.Contains('\r')
               || value.Contains('\n');
    }
}
