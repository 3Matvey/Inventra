using Inventra.Application.Identity;

namespace Inventra.Application.Common.Interfaces;

public interface IExternalIdentityService
{
    Task<ExternalUserInfo?> CompleteSignInAsync(CancellationToken cancellationToken = default);
}
