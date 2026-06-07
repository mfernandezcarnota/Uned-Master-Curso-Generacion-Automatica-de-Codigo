namespace RpgCharacterBuilder.Domain;

public sealed record CharacterClass : NamedValue
{
    private CharacterClass(string name) : base(name)
    {
    }

    public static CharacterClass Warrior { get; } = new("Warrior");
    public static CharacterClass Mage { get; } = new("Mage");
    public static CharacterClass Archer { get; } = new("Archer");

    public static CharacterClass Custom(string name) => new(name);
}
