namespace RpgCharacterBuilder.Domain;

public sealed record Race : NamedValue
{
    private Race(string name) : base(name)
    {
    }

    public static Race Human { get; } = new("Human");
    public static Race Elf { get; } = new("Elf");
    public static Race Dwarf { get; } = new("Dwarf");
    public static Race Orc { get; } = new("Orc");

    public static Race Custom(string name) => new(name);
}
