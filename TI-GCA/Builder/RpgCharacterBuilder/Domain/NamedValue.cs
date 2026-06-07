namespace RpgCharacterBuilder.Domain;

/// <summary>
/// Represents a named RPG concept while keeping the domain open to values
/// introduced by future game modules.
/// </summary>
public abstract record NamedValue
{
    protected NamedValue(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
    }

    public string Name { get; }

    public override string ToString() => Name;
}
