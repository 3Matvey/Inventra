namespace Inventra.Application.Identity.BeginExternalLogin;

public sealed record BeginExternalLoginRequest(
    string Provider,
    string ReturnUrl);
