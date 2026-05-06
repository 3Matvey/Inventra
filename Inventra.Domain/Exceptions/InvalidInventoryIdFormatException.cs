namespace Inventra.Domain.Exceptions;

public sealed class InvalidInventoryIdFormatException(string message)
    : DomainException(message);

