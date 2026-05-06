namespace Inventra.Domain.Exceptions;

public sealed class InvalidItemFieldValueException(string message) 
    : DomainException(message);