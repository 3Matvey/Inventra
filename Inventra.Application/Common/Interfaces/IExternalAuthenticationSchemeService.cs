namespace Inventra.Application.Common.Interfaces;

/// <summary>
/// Provides information about configured external authentication schemes.
/// </summary>
public interface IExternalAuthenticationSchemeService
{
    Task<bool> IsConfiguredAsync(string provider);
}
