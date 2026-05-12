namespace Inventra.Application.Identity.Admin.ChangeUserBlockStatus;

public sealed record ChangeUserBlockStatusRequest(Guid UserId, bool IsBlocked);
