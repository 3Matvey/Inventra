namespace Inventra.Application.Common.Interfaces;

public interface IAuthenticationSession
{
    Task SignOutAsync();
}
