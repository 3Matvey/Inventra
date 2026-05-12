namespace Inventra.Application.Identity.Admin.ChangeUserAdminRole;

public sealed record ChangeUserAdminRoleRequest(Guid UserId, bool IsAdmin);
