using Inventra.Domain.Enums;

namespace Inventra.Domain.Exceptions;

public sealed class InventoryFieldLimitExceededException(InventoryFieldType fieldType, int limit)
    : DomainException($"Inventory cannot contain more than {limit} fields of type '{fieldType}'.");

