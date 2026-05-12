using Inventra.Application.Identity.Admin.ChangeUserAdminRole;
using Inventra.Application.Identity.Admin.ChangeUserBlockStatus;
using Inventra.Application.Identity.Admin.DeleteUser;
using Inventra.Application.Identity.Admin.GetUsersPage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/users")]
public class AdminUsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromServices] GetUsersPageUseCase useCase,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(
            new GetUsersPageRequest(page, pageSize),
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpPost("{userId:guid}/block")]
    public async Task<IActionResult> BlockUser(
        Guid userId,
        [FromServices] ChangeUserBlockStatusUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ChangeUserBlockStatusRequest(userId, IsBlocked: true),
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpPost("{userId:guid}/unblock")]
    public async Task<IActionResult> UnblockUser(
        Guid userId,
        [FromServices] ChangeUserBlockStatusUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ChangeUserBlockStatusRequest(userId, IsBlocked: false),
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpPost("{userId:guid}/admin-role")]
    public async Task<IActionResult> AddAdminRole(
        Guid userId,
        [FromServices] ChangeUserAdminRoleUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ChangeUserAdminRoleRequest(userId, IsAdmin: true),
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpDelete("{userId:guid}/admin-role")]
    public async Task<IActionResult> RemoveAdminRole(
        Guid userId,
        [FromServices] ChangeUserAdminRoleUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ChangeUserAdminRoleRequest(userId, IsAdmin: false),
            cancellationToken);

        return ToActionResult(result);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(
        Guid userId,
        [FromServices] DeleteUserUseCase useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new DeleteUserRequest(userId),
            cancellationToken);

        return ToActionResult(result);
    }
}
