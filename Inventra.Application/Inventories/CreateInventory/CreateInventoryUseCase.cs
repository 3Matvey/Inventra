using Inventra.Application.Common.Interfaces;
using Inventra.Application.Common.Results;
using Inventra.Domain.Entities;

namespace Inventra.Application.Inventories.CreateInventory;

public sealed class CreateInventoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryUseCase(
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider,
        IInventoryRepository inventoryRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _inventoryRepository = inventoryRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> ExecuteAsync(
        CreateInventoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return InventoryErrors.AuthenticationRequired();

        if (!await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
            return InventoryErrors.CategoryNotFound(request.CategoryId);

        var inventory = CreateInventory(request, _currentUser.UserId.Value);

        await _inventoryRepository.AddAsync(inventory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return inventory.Id;
    }

    private Inventory CreateInventory(CreateInventoryRequest request, Guid ownerId)
    {
        return new Inventory(
            ownerId,
            request.CategoryId,
            request.Title,
            request.DescriptionMarkdown,
            request.ImageUrl,
            _dateTimeProvider.UtcNow);
    }
}
