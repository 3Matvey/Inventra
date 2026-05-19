using Inventra.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Inventra.Infrastructure.Identity;

internal class IdentityAccountService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IBlockedUserSessionCache blockedUserSessionCache) : IIdentityAccountService
{
    public async Task SetAdminRoleAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return;

        await EnsureAdminRoleAsync();

        if (isAdmin && !await userManager.IsInRoleAsync(user, IdentityRoles.Admin))
            await userManager.AddToRoleAsync(user, IdentityRoles.Admin);

        if (!isAdmin && await userManager.IsInRoleAsync(user, IdentityRoles.Admin))
            await userManager.RemoveFromRoleAsync(user, IdentityRoles.Admin);
    }

    public async Task SetBlockedAsync(
        Guid userId,
        bool isBlocked,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return;

        user.LockoutEnabled = true;
        user.LockoutEnd = isBlocked ? DateTimeOffset.MaxValue : null;

        await userManager.UpdateAsync(user);
        await UpdateBlockedSessionCacheAsync(userId, isBlocked, cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is not null)
            await userManager.DeleteAsync(user);
    }

    private async Task EnsureAdminRoleAsync()
    {
        if (!await roleManager.RoleExistsAsync(IdentityRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole<Guid>(IdentityRoles.Admin));
    }

    private async Task UpdateBlockedSessionCacheAsync(
        Guid userId,
        bool isBlocked,
        CancellationToken cancellationToken)
    {
        if (isBlocked)
            await blockedUserSessionCache.MarkBlockedAsync(userId, cancellationToken);
        else
            await blockedUserSessionCache.MarkUnblockedAsync(userId, cancellationToken);
    }
}
