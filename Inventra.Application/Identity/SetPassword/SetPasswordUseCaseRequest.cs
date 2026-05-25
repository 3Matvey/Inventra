namespace Inventra.Application.Identity.SetPassword;

public sealed record SetPasswordUseCaseRequest(
    Guid UserId,
    string Token,
    string Password);
