using Inventra.Application.Identity;
using Inventra.Application.Inventories.Comments.Dto;

namespace Inventra.Application.Inventories.AddInventoryComment;

public sealed class AddInventoryCommentUseCase(
    ICurrentUser currentUser,
    IInventoryRepository inventoryRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IUseCase
{
    public async Task<Result<InventoryCommentDto>> ExecuteAsync(
        AddInventoryCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
            return InventoryErrors.AuthenticationRequired();

        var inventory = await inventoryRepository.GetByIdAsync(request.InventoryId, cancellationToken);

        return inventory is null
            ? InventoryErrors.NotFound(request.InventoryId)
            : await AddAsync(request, inventory, currentUser.UserId.Value, cancellationToken);
    }

    private async Task<Result<InventoryCommentDto>> AddAsync(
        AddInventoryCommentRequest request,
        Inventory inventory,
        Guid authorId,
        CancellationToken cancellationToken)
    {
        var author = await userRepository.GetByIdAsync(authorId, cancellationToken);

        return author is null
            ? IdentityErrors.UserNotFound(authorId)
            : await SaveAsync(inventory, author, request.BodyMarkdown, cancellationToken);
    }

    private async Task<InventoryCommentDto> SaveAsync(
        Inventory inventory,
        UserAccount author,
        string bodyMarkdown,
        CancellationToken cancellationToken)
    {
        var comment = inventory.AddComment(author.Id, bodyMarkdown);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(comment, author);
    }

    private static InventoryCommentDto ToDto(
        InventoryComment comment,
        UserAccount author)
    {
        return new InventoryCommentDto(
            comment.Id,
            comment.InventoryId,
            comment.AuthorId,
            author.UserName,
            comment.BodyMarkdown,
            comment.CreatedAt);
    }
}
