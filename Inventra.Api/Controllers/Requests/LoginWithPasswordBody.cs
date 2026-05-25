using Inventra.Application.Identity.LoginWithPassword;

namespace Inventra.Api.Controllers.Requests;

public sealed record LoginWithPasswordBody(
    string Email,
    string Password,
    bool RememberMe)
{
    public LoginWithPasswordRequest ToRequest()
    {
        return new LoginWithPasswordRequest(
            Email,
            Password,
            RememberMe);
    }
}
