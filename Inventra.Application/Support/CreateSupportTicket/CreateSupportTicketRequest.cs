namespace Inventra.Application.Support.CreateSupportTicket;

public sealed record CreateSupportTicketRequest(
    string Summary,
    string Priority,
    string Link,
    IReadOnlyCollection<string> AdminEmails,
    Guid? InventoryId = null);
