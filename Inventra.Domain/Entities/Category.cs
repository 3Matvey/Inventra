using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class Category : Entity
{
    public string Name { get; private set; } = string.Empty;

    private Category()
    {
    }

    public Category(string name)
    {
        Rename(name);
    }

    public void Rename(string name)
    {
        Name = Guard.Required(name);
    }
}
