namespace Inventra.Application.Support.CreateSupportTicket;

internal static class SupportTicketErrors
{
    public static Error AuthenticationRequired()
        => Error.AccessUnauthorized(
            "SupportTicket.AuthenticationRequired",
            "Authentication is required to create a support ticket.");

    public static Error InvalidSummary()
        => Error.BadRequest(
            "SupportTicket.InvalidSummary",
            "Support ticket summary is required.");

    public static Error InvalidPriority()
        => Error.BadRequest(
            "SupportTicket.InvalidPriority",
            "Priority must be High, Average, or Low.");

    public static Error InvalidLink()
        => Error.BadRequest(
            "SupportTicket.InvalidLink",
            "A valid support ticket link is required.");

    public static Error AdminEmailsRequired()
        => Error.BadRequest(
            "SupportTicket.AdminEmailsRequired",
            "At least one admin email is required.");

    public static Error InventoryNotFound(Guid inventoryId)
        => Error.NotFound(
            "SupportTicket.InventoryNotFound",
            $"Inventory '{inventoryId}' was not found.");
}
