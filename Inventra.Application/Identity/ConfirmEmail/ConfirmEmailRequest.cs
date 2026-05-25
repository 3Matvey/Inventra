namespace Inventra.Application.Identity.ConfirmEmail;

public sealed record ConfirmEmailRequest(
    Guid UserId,
    string Token);
