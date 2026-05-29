using System.Text.Json;

namespace Inventra.Application.Support.CreateSupportTicket;

public sealed class CreateSupportTicketUseCase(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IInventoryRepository inventoryRepository,
    ISupportTicketFileStorage fileStorage,
    TimeProvider timeProvider)
    : IUseCase
{
    private static readonly string[] Priorities = ["High", "Average", "Low"];

    public async Task<Result> ExecuteAsync(
        CreateSupportTicketRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (!validation.IsSuccess)
            return validation.Error;

        return await CreateAsync(request, cancellationToken);
    }

    private async Task<Result> CreateAsync(
        CreateSupportTicketRequest request,
        CancellationToken cancellationToken)
    {
        var payloadResult = await BuildPayloadAsync(request, cancellationToken);
        if (!payloadResult.IsSuccess)
            return payloadResult.Error;

        var json = JsonSerializer.Serialize(payloadResult.Value);
        return await fileStorage.UploadAsync(CreateFileName(), json, cancellationToken);
    }

    private Result Validate(CreateSupportTicketRequest request)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return SupportTicketErrors.AuthenticationRequired();

        if (string.IsNullOrWhiteSpace(request.Summary))
            return SupportTicketErrors.InvalidSummary();

        if (!Priorities.Contains(request.Priority, StringComparer.OrdinalIgnoreCase))
            return SupportTicketErrors.InvalidPriority();

        return ValidateLinkAndEmails(request);
    }

    private static Result ValidateLinkAndEmails(CreateSupportTicketRequest request)
    {
        if (!Uri.TryCreate(request.Link, UriKind.Absolute, out _))
            return SupportTicketErrors.InvalidLink();

        return request.AdminEmails.Any(email => !string.IsNullOrWhiteSpace(email))
            ? Result.Success()
            : SupportTicketErrors.AdminEmailsRequired();
    }

    private async Task<Result<SupportTicketPayload>> BuildPayloadAsync(
        CreateSupportTicketRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId!.Value, cancellationToken);

        if (request.InventoryId is null)
            return ToPayload(request, user, null);

        var inventoryTitle = await GetInventoryTitleAsync(request.InventoryId.Value, cancellationToken);

        return inventoryTitle.IsSuccess
            ? ToPayload(request, user, inventoryTitle.Value)
            : inventoryTitle.Error;
    }

    private async Task<Result<string>> GetInventoryTitleAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        var inventory = await inventoryRepository.GetByIdAsync(inventoryId, cancellationToken);

        return inventory is null
            ? SupportTicketErrors.InventoryNotFound(inventoryId)
            : inventory.Title;
    }

    private SupportTicketPayload ToPayload(
        CreateSupportTicketRequest request,
        UserAccount? user,
        string? inventoryTitle)
    {
        return new SupportTicketPayload(
            FormatReporter(user),
            inventoryTitle,
            request.Link.Trim(),
            NormalizePriority(request.Priority),
            [.. request.AdminEmails.Select(x => x.Trim())],
            request.Summary.Trim(),
            timeProvider.GetUtcNow());
    }

    private static string FormatReporter(UserAccount? user) 
        => user is null
            ? "Unknown authenticated user"
            : $"{user.UserName} <{user.Email}>";

    private static string NormalizePriority(string priority)
    {
        return Priorities.First(x => string.Equals(
            x,
            priority,
            StringComparison.OrdinalIgnoreCase));
    }

    private string CreateFileName()
    {
        var timestamp = timeProvider.GetUtcNow().ToString("yyyyMMddHHmmss");

        return $"support-ticket-{timestamp}-{Guid.CreateVersion7():N}.json";
    }
}
