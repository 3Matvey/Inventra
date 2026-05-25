namespace Inventra.Application.Identity.LoginWithPassword;

public sealed record LoginWithPasswordRequest(
    string Email,
    string Password,
    bool RememberMe);
