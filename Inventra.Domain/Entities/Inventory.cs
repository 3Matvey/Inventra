using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class Inventory : AuditableEntity
{
    public const int MaxFieldsPerType = 3;
    public const int RecommendedMaxIdElements = 10;

    private readonly List<InventoryField> _fields = [];
    private readonly List<InventoryAccessGrant> _accessGrants = [];
    private readonly List<InventoryIdFormatElement> _idFormatElements = [];
    private readonly List<InventoryTag> _tags = [];
    private readonly List<InventoryComment> _comments = [];

    public Guid OwnerId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? DescriptionMarkdown { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsPublicWriteAccess { get; private set; }
    public long Version { get; private set; }

    public IReadOnlyCollection<InventoryField> Fields => _fields.AsReadOnly();
    public IReadOnlyCollection<InventoryAccessGrant> AccessGrants => _accessGrants.AsReadOnly();
    public IReadOnlyCollection<InventoryIdFormatElement> IdFormatElements => _idFormatElements.AsReadOnly();
    public IReadOnlyCollection<InventoryTag> Tags => _tags.AsReadOnly();
    public IReadOnlyCollection<InventoryComment> Comments => _comments.AsReadOnly();

    private Inventory()
    {
    }

    public Inventory(
        Guid ownerId,
        Guid categoryId,
        string title,
        string? descriptionMarkdown,
        string? imageUrl,
        DateTimeOffset createdAt)
        : base(createdAt)
    {
        OwnerId = Guard.RequiredId(ownerId);
        CategoryId = Guard.RequiredId(categoryId);
        SetSettings(title, descriptionMarkdown, categoryId, imageUrl, createdAt, incrementVersion: false);
    }

    public void UpdateSettings(
        string title,
        string? descriptionMarkdown,
        Guid categoryId,
        string? imageUrl,
        DateTimeOffset changedAt)
    {
        SetSettings(title, descriptionMarkdown, categoryId, imageUrl, changedAt, incrementVersion: true);
    }

    public void SetPublicWriteAccess(bool isPublic, DateTimeOffset changedAt)
    {
        IsPublicWriteAccess = isPublic;
        MarkChanged(changedAt);
    }

    public InventoryAccessGrant GrantAccess(Guid userId, DateTimeOffset grantedAt)
    {
        userId = Guard.RequiredId(userId);

        if (userId == OwnerId)
        {
            throw new InvalidOperationException("Owner does not need an explicit access grant.");
        }

        var existingGrant = _accessGrants.SingleOrDefault(x => x.UserId == userId);

        if (existingGrant is not null)
            return existingGrant;

        var grant = new InventoryAccessGrant(Id, userId, grantedAt);
        _accessGrants.Add(grant);
        MarkChanged(grantedAt);

        return grant;
    }

    public void RevokeAccess(Guid userId, DateTimeOffset changedAt)
    {
        userId = Guard.RequiredId(userId);

        var grant = _accessGrants.SingleOrDefault(x => x.UserId == userId);

        if (grant is null)
            return;

        _accessGrants.Remove(grant);
        MarkChanged(changedAt);
    }

    public bool HasExplicitWriteAccess(Guid userId)
    {
        userId = Guard.RequiredId(userId);

        return userId == OwnerId || _accessGrants.Any(x => x.UserId == userId);
    }

    public InventoryField AddField(
        InventoryFieldType type,
        string title,
        string? description,
        bool showInTable,
        DateTimeOffset changedAt)
    {
        EnsureFieldLimit(type);

        var field = new InventoryField(
            Id,
            type,
            title,
            description,
            showInTable,
            NextFieldOrder());

        _fields.Add(field);
        MarkChanged(changedAt);

        return field;
    }

    public void UpdateField(Guid fieldId, string title, string? description, bool showInTable, DateTimeOffset changedAt)
    {
        fieldId = Guard.RequiredId(fieldId);

        var field = GetField(fieldId);
        field.Update(title, description, showInTable);
        MarkChanged(changedAt);
    }

    public void RemoveField(Guid fieldId, DateTimeOffset changedAt)
    {
        fieldId = Guard.RequiredId(fieldId);

        var field = GetField(fieldId);
        _fields.Remove(field);
        NormalizeFieldOrder();
        MarkChanged(changedAt);
    }

    public void ReorderFields(IReadOnlyList<Guid> orderedFieldIds, DateTimeOffset changedAt)
    {
        orderedFieldIds = Guard.RequiredCompleteIdSet(orderedFieldIds, _fields.Count);

        for (int i = 0; i < orderedFieldIds.Count; i++)
        {
            GetField(orderedFieldIds[i]).MoveTo(i);
        }

        MarkChanged(changedAt);
    }

    public InventoryIdFormatElement AddIdFormatElement(
        InventoryIdElementType type,
        string? value,
        string? format,
        DateTimeOffset changedAt)
    {
        if (_idFormatElements.Count >= RecommendedMaxIdElements)
        {
            throw new InvalidInventoryIdFormatException(
                $"Inventory ID format cannot contain more than {RecommendedMaxIdElements} elements.");
        }

        var element = new InventoryIdFormatElement(
            Id,
            type,
            value,
            format,
            NextIdElementOrder());

        _idFormatElements.Add(element);
        MarkChanged(changedAt);

        return element;
    }

    public void UpdateIdFormatElement(Guid elementId, string? value, string? format, DateTimeOffset changedAt)
    {
        elementId = Guard.RequiredId(elementId);

        var element = GetIdFormatElement(elementId);
        element.Update(value, format);
        MarkChanged(changedAt);
    }

    public void RemoveIdFormatElement(Guid elementId, DateTimeOffset changedAt)
    {
        elementId = Guard.RequiredId(elementId);

        var element = GetIdFormatElement(elementId);
        _idFormatElements.Remove(element);
        NormalizeIdElementOrder();
        MarkChanged(changedAt);
    }

    public void ReorderIdFormatElements(IReadOnlyList<Guid> orderedElementIds, DateTimeOffset changedAt)
    {
        orderedElementIds = Guard.RequiredCompleteIdSet(orderedElementIds, _idFormatElements.Count);

        for (int i = 0; i < orderedElementIds.Count; i++)
        {
            GetIdFormatElement(orderedElementIds[i]).MoveTo(i);
        }

        MarkChanged(changedAt);
    }

    public void ReplaceTags(IEnumerable<Guid> tagIds, DateTimeOffset changedAt)
    {
        var distinctTagIds = Guard.RequiredIds(tagIds);

        _tags.Clear();
        _tags.AddRange(distinctTagIds.Select(tagId => new InventoryTag(Id, tagId)));

        MarkChanged(changedAt);
    }

    public InventoryComment AddComment(Guid authorId, string bodyMarkdown, DateTimeOffset createdAt)
    {
        authorId = Guard.RequiredId(authorId);

        var comment = new InventoryComment(Id, authorId, bodyMarkdown, createdAt);
        _comments.Add(comment);

        return comment;
    }

    private void SetSettings(
        string title,
        string? descriptionMarkdown,
        Guid categoryId,
        string? imageUrl,
        DateTimeOffset changedAt,
        bool incrementVersion)
    {
        Title = Guard.Required(title);
        CategoryId = Guard.RequiredId(categoryId);
        DescriptionMarkdown = Guard.Optional(descriptionMarkdown);
        ImageUrl = Guard.Optional(imageUrl);

        if (incrementVersion)
        {
            MarkChanged(changedAt);
        }
    }

    private void EnsureFieldLimit(InventoryFieldType type)
    {
        if (_fields.Count(x => x.Type == type) >= MaxFieldsPerType)
        {
            throw new InventoryFieldLimitExceededException(type, MaxFieldsPerType);
        }
    }

    private InventoryField GetField(Guid fieldId)
    {
        return _fields.SingleOrDefault(x => x.Id == fieldId)
            ?? throw new KeyNotFoundException($"Inventory field '{fieldId}' was not found.");
    }

    private InventoryIdFormatElement GetIdFormatElement(Guid elementId)
    {
        return _idFormatElements.SingleOrDefault(x => x.Id == elementId)
            ?? throw new KeyNotFoundException($"Inventory ID format element '{elementId}' was not found.");
    }

    private int NextFieldOrder() => _fields.Count == 0 ? 0 : _fields.Max(x => x.Order) + 1;

    private int NextIdElementOrder() => _idFormatElements.Count == 0 ? 0 : _idFormatElements.Max(x => x.Order) + 1;

    private void NormalizeFieldOrder()
    {
        var orderedFields = _fields.OrderBy(x => x.Order).ToArray();

        for (var index = 0; index < orderedFields.Length; index++)
        {
            orderedFields[index].MoveTo(index);
        }
    }

    private void NormalizeIdElementOrder()
    {
        var orderedElements = _idFormatElements.OrderBy(x => x.Order).ToArray();

        for (var index = 0; index < orderedElements.Length; index++)
        {
            orderedElements[index].MoveTo(index);
        }
    }

    private void MarkChanged(DateTimeOffset changedAt)
    {
        Version++;
        Touch(changedAt);
    }
}
