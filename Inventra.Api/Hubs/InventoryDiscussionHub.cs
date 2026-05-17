using Microsoft.AspNetCore.SignalR;

namespace Inventra.Api.Hubs;

public class InventoryDiscussionHub : Hub
{
    public async Task JoinInventoryDiscussion(Guid inventoryId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            InventoryDiscussionGroups.ForInventory(inventoryId));
    }

    public async Task LeaveInventoryDiscussion(Guid inventoryId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            InventoryDiscussionGroups.ForInventory(inventoryId));
    }
}
