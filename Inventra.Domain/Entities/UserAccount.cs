using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class UserAccount : AuditableEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsBlocked { get; private set; }
    public bool IsAdmin { get; private set; }

    private UserAccount()
    {
    }

    public UserAccount(string userName, string email)
    {
        Rename(userName);
        ChangeEmail(email);
    }

    public UserAccount(Guid id, string userName, string email)
        : this(userName, email)
    {
        Id = Guard.RequiredId(id);
    }

    public void Rename(string userName)
    {
        UserName = Guard.Required(userName);
    }

    public void ChangeEmail(string email)
    {
        Email = Guard.Required(email);
    }

    public void Block() => IsBlocked = true;

    public void Unblock() => IsBlocked = false;

    public void AddAdminRole() => IsAdmin = true;

    public void RemoveAdminRole() => IsAdmin = false;
}
