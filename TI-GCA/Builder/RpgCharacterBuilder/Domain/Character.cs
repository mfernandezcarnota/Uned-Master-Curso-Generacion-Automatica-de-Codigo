using System.Collections.ObjectModel;

namespace RpgCharacterBuilder.Domain;

public sealed class Character
{
    public Character(
        string name,
        Race race,
        CharacterClass characterClass,
        Weapon? weapon,
        Armor? armor,
        IEnumerable<Skill> skills,
        IEnumerable<SpecialAttribute> specialAttributes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The character name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Race = race ?? throw new ArgumentNullException(nameof(race));
        Class = characterClass ?? throw new ArgumentNullException(nameof(characterClass));
        Weapon = weapon;
        Armor = armor;
        Skills = new ReadOnlyCollection<Skill>(skills?.ToList() ?? throw new ArgumentNullException(nameof(skills)));
        SpecialAttributes = new ReadOnlyCollection<SpecialAttribute>(specialAttributes?.ToList() ?? throw new ArgumentNullException(nameof(specialAttributes)));
    }

    public string Name { get; }

    public Race Race { get; }

    public CharacterClass Class { get; }

    public Weapon? Weapon { get; }

    public Armor? Armor { get; }

    public IReadOnlyCollection<Skill> Skills { get; }

    public IReadOnlyCollection<SpecialAttribute> SpecialAttributes { get; }
}
