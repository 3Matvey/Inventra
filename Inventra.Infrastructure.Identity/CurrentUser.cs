using System.Security.Claims;
using Inventra.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Inventra.Infrastructure.Identity;

internal class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public bool IsAuthenticated =>
        User.Identity?.IsAuthenticated == true;

    public bool IsAdmin =>
        User.IsInRole(IdentityRoles.Admin);

    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
}
