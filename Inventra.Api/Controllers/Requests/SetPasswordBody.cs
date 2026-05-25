using Inventra.Application.Identity.SetPassword;

namespace Inventra.Api.Controllers.Requests;

public sealed record SetPasswordBody(
    Guid UserId,
    string Token,
    string Password)
{
    public SetPasswordUseCaseRequest ToRequest()
    {
        return new SetPasswordUseCaseRequest(
            UserId,
            Token,
            Password);
    }
}
