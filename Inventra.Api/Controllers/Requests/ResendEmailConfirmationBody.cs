using Inventra.Application.Identity.ResendEmailConfirmation;

namespace Inventra.Api.Controllers.Requests;

public sealed record ResendEmailConfirmationBody(string Email)
{
    public ResendEmailConfirmationRequest ToRequest()
    {
        return new ResendEmailConfirmationRequest(Email);
    }
}
