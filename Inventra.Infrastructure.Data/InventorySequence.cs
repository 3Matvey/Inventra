namespace Inventra.Infrastructure.Data;

public class InventorySequence
{
    public Guid InventoryId { get; set; }

    public long NextValue { get; set; }
}
