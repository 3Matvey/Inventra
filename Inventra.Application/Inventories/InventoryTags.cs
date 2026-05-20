namespace Inventra.Application.Inventories;

internal static class InventoryTags
{
    public static async Task<IReadOnlyCollection<Tag>> ResolveAsync(
        ITagRepository tagRepository,
        IReadOnlyCollection<string> tagNames,
        CancellationToken cancellationToken)
    {
        var names = Normalize(tagNames);
        var existingTags = await tagRepository.GetByNamesAsync(names, cancellationToken);
        var missingTags = await AddMissingTagsAsync(tagRepository, names, existingTags, cancellationToken);

        return existingTags.Concat(missingTags).ToArray();
    }

    private static async Task<IReadOnlyCollection<Tag>> AddMissingTagsAsync(
        ITagRepository tagRepository,
        IReadOnlyCollection<string> names,
        IReadOnlyCollection<Tag> existingTags,
        CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        foreach (var name in MissingNames(names, existingTags))
            tags.Add(await AddTagAsync(tagRepository, name, cancellationToken));

        return tags;
    }

    private static async Task<Tag> AddTagAsync(
        ITagRepository tagRepository,
        string name,
        CancellationToken cancellationToken)
    {
        var tag = new Tag(name);
        await tagRepository.AddAsync(tag, cancellationToken);

        return tag;
    }

    private static IEnumerable<string> MissingNames(
        IEnumerable<string> names,
        IEnumerable<Tag> existingTags)
    {
        var existingNames = existingTags
            .Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return names.Where(name => !existingNames.Contains(name));
    }

    private static string[] Normalize(IEnumerable<string> tagNames)
    {
        return [.. tagNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)];
    }
}
