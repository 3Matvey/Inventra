using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class Tag : Entity
{
    public string Name { get; private set; } = string.Empty;

    private Tag()
    {
    }

    public Tag(string name)
    {
        Rename(name);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name);
    }
}
