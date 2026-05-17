namespace Inventra.Api.Controllers.Requests;

public sealed record GrantInventoryAccessBody(
    long ExpectedVersion,
    string UserNameOrEmail);
