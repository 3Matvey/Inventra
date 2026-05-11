using Inventra.Domain.Common;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class InventoryItem : AuditableEntity
{
    private readonly List<ItemFieldValue> _fieldValues = [];
    private readonly List<ItemLike> _likes = [];

    public Guid InventoryId { get; private set; }
    public Guid CreatedById { get; private set; }
    public string CustomId { get; private set; } = string.Empty;
    public long? SequenceNumber { get; private set; }
    public long Version { get; private set; }

    public IReadOnlyCollection<ItemFieldValue> FieldValues => _fieldValues.AsReadOnly();
    public IReadOnlyCollection<ItemLike> Likes => _likes.AsReadOnly();

    private InventoryItem()
    {
    }

    public InventoryItem(
        Guid inventoryId,
        Guid createdById,
        string customId,
        long? sequenceNumber,
        DateTimeOffset createdAt)
        : base(createdAt)
    {
        InventoryId = Guard.RequiredId(inventoryId);
        CreatedById = Guard.RequiredId(createdById);
        SequenceNumber = sequenceNumber;
        ChangeCustomId(customId, createdAt);
    }

    public void ChangeCustomId(string customId, DateTimeOffset changedAt)
    {
        CustomId = Guard.Required(customId);
        MarkChanged(changedAt);
    }

    public void SetFieldValue(InventoryField field, FieldValue value, DateTimeOffset changedAt)
    {
        field = Guard.Required(field);
        value = Guard.Required(value);

        if (field.InventoryId != InventoryId)
        {
            throw new InvalidOperationException("Field belongs to another inventory.");
        }

        var existingValue = _fieldValues.SingleOrDefault(x => x.FieldId == field.Id);

        if (existingValue is null)
        {
            _fieldValues.Add(new ItemFieldValue(Id, field.Id, field.Type, value));
        }
        else
        {
            existingValue.SetValue(field.Type, value);
        }

        MarkChanged(changedAt);
    }

    public void Like(Guid userId, DateTimeOffset createdAt)
    {
        userId = Guard.RequiredId(userId);
        
        if (_likes.Any(x => x.UserId == userId))
        {
            return;
        }

        _likes.Add(new ItemLike(Id, userId, createdAt));
    }

    public void Unlike(Guid userId)
    {
        userId = Guard.RequiredId(userId);

        var like = _likes.SingleOrDefault(x => x.UserId == userId);

        if (like is not null)
        {
            _likes.Remove(like);
        }
    }

    private void MarkChanged(DateTimeOffset changedAt)
    {
        Version++;
        Touch(changedAt);
    }
}
