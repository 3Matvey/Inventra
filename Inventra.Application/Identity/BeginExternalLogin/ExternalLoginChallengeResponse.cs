namespace Inventra.Application.Identity.BeginExternalLogin;

public sealed record ExternalLoginChallengeResponse(
    string Provider,
    string RedirectUri);
