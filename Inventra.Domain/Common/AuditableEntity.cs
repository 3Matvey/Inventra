namespace Inventra.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(DateTimeOffset createdAt)
    {
        CreatedAt = createdAt;
    }

    protected void Touch(DateTimeOffset changedAt)
    {
        UpdatedAt = changedAt;
    }
}
