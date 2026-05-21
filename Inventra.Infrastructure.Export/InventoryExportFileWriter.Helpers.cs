using Inventra.Application.Inventories.Exports;
using Inventra.Application.Inventories.Exports.Dto;
using Inventra.Domain.Enums;
using System.Globalization;

namespace Inventra.Infrastructure.Export;

internal sealed partial class InventoryExportFileWriter
{
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
