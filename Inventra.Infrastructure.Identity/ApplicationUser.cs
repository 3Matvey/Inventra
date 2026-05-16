using Microsoft.AspNetCore.Identity;

namespace Inventra.Infrastructure.Identity;

/// <summary>
/// Represent a user in the Inventra identity system
/// </summary>
internal class ApplicationUser : IdentityUser<Guid>
{
}
