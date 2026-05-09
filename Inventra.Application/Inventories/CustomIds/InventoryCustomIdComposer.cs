using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;

namespace Inventra.Application.Inventories.CustomIds;

internal static class InventoryCustomIdComposer
{
    private const long SampleSequence = 42;

    public static string Compose(
        IEnumerable<InventoryIdFormatElement> elements,
        CustomIdComposeContext context)
    {
        var builder = new StringBuilder();

        foreach (var element in elements.OrderBy(x => x.Order))
            builder.Append(ComposeElement(element, context));

        return builder.ToString();
    }

    public static CustomIdComposeContext PreviewContext(DateTimeOffset now) =>
        new(now, SampleSequence, UseSampleValues: true);

    private static string ComposeElement(
        InventoryIdFormatElement element,
        CustomIdComposeContext context)
    {
        return element.Type switch
        {
            InventoryIdElementType.FixedText => element.Value ?? string.Empty,
            InventoryIdElementType.Random20BitNumber => Random20Bit(element, context),
            InventoryIdElementType.Random32BitNumber => Random32Bit(element, context),
            InventoryIdElementType.Random6DigitNumber => RandomDigits(element, context, 123456, 1_000_000, "D6"),
            InventoryIdElementType.Random9DigitNumber => RandomDigits(element, context, 123456789, 1_000_000_000, "D9"),
            InventoryIdElementType.Guid => GuidValue(element, context),
            InventoryIdElementType.DateTime => DateTimeValue(element, context),
            InventoryIdElementType.Sequence => Number(context.Sequence, element.Format),
            _ => string.Empty
        };
    }

    private static string Random20Bit(
        InventoryIdFormatElement element,
        CustomIdComposeContext context)
    {
        var value = context.UseSampleValues
            ? 1_048_575
            : RandomNumberGenerator.GetInt32(1 << 20);

        return Number(value, element.Format);
    }

    private static string Random32Bit(
        InventoryIdFormatElement element,
        CustomIdComposeContext context)
    {
        var value = context.UseSampleValues ? uint.MaxValue : RandomUInt32();

        return Number(value, element.Format);
    }

    private static string RandomDigits(
        InventoryIdFormatElement element,
        CustomIdComposeContext context,
        int sample,
        int exclusiveMax,
        string defaultFormat)
    {
        var value = context.UseSampleValues
            ? sample
            : RandomNumberGenerator.GetInt32(exclusiveMax);

        return Number(value, element.Format ?? defaultFormat);
    }

    private static string GuidValue(
        InventoryIdFormatElement element,
        CustomIdComposeContext context)
    {
        var value = context.UseSampleValues
            ? Guid.Parse("12345678-1234-1234-1234-123456789abc")
            : Guid.NewGuid();

        return value.ToString(element.Format ?? "D", CultureInfo.InvariantCulture);
    }

    private static string DateTimeValue(
        InventoryIdFormatElement element,
        CustomIdComposeContext context)
    {
        return context.CreatedAt.ToString(
            element.Format ?? "yyyyMMddHHmmss",
            CultureInfo.InvariantCulture);
    }

    private static uint RandomUInt32()
    {
        Span<byte> bytes = stackalloc byte[sizeof(uint)];
        RandomNumberGenerator.Fill(bytes);

        return BitConverter.ToUInt32(bytes);
    }

    private static string Number<T>(T value, string? format)
        where T : IFormattable
    {
        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}
