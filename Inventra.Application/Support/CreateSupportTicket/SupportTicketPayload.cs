namespace Inventra.Application.Support.CreateSupportTicket;

internal sealed record SupportTicketPayload(
    string ReportedBy,
    string? Inventory,
    string Link,
    string Priority,
    IReadOnlyCollection<string> AdminEmails,
    string Summary,
    DateTimeOffset CreatedAtUtc);
