namespace Inventra.Api.Controllers.Requests;

public sealed record SetPublicWriteAccessBody(long ExpectedVersion, bool IsPublic);
