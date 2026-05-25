using Inventra.Application.Identity.RegisterWithPassword;

namespace Inventra.Api.Controllers.Requests;

public sealed record RegisterWithPasswordBody(
    string UserName,
    string Email,
    string Password)
{
    public RegisterWithPasswordRequest ToRequest()
    {
        return new RegisterWithPasswordRequest(
            UserName,
            Email,
            Password);
    }
}
