namespace Inventra.Application.Identity.RegisterWithPassword;

public sealed record RegisterWithPasswordRequest(
    string UserName,
    string Email,
    string Password);
