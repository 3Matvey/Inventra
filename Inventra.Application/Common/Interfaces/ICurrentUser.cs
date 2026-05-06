namespace Inventra.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }

    bool IsAuthenticated { get; }

    bool IsAdmin { get; }
}
