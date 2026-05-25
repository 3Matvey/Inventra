using Inventra.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Inventra.Infrastructure.Identity;

internal sealed class ExternalAuthenticationSchemeService(
    IAuthenticationSchemeProvider schemes) : IExternalAuthenticationSchemeService
{
    public async Task<bool> IsConfiguredAsync(string provider)
    {
        return await schemes.GetSchemeAsync(provider) is not null;
    }
}
