namespace Inventra.Application.Inventories;

internal static class InventoryOrderReorder
{
    public static void MoveFieldsToTemporaryOrders(Inventory inventory)
    {
        var fields = inventory.Fields.ToArray();
        var offset = NextTemporaryOrder(fields.Select(x => x.Order));

        for (var index = 0; index < fields.Length; index++)
            fields[index].MoveTo(offset + index);
    }

    public static void MoveIdElementsToTemporaryOrders(Inventory inventory)
    {
        var elements = inventory.IdFormatElements.ToArray();
        var offset = NextTemporaryOrder(elements.Select(x => x.Order));

        for (var index = 0; index < elements.Length; index++)
            elements[index].MoveTo(offset + index);
    }

    private static int NextTemporaryOrder(IEnumerable<int> orders)
    {
        return orders.DefaultIfEmpty(-1).Max() + 1;
    }
}
