using RpgCharacterBuilder.Domain;

namespace RpgCharacterBuilder.Building;

public sealed class RpgCharacterBuilder : ICharacterBuilder
{
    private readonly List<Skill> _skills = [];
    private readonly List<SpecialAttribute> _specialAttributes = [];
    private string? _name;
    private Race? _race;
    private CharacterClass? _class;
    private Weapon? _weapon;
    private Armor? _armor;

    public ICharacterBuilder Named(string name)
    {
        _name = RequireText(name, nameof(name));
        return this;
    }

    public ICharacterBuilder OfRace(Race race)
    {
        _race = race ?? throw new ArgumentNullException(nameof(race));
        return this;
    }

    public ICharacterBuilder WithClass(CharacterClass characterClass)
    {
        _class = characterClass ?? throw new ArgumentNullException(nameof(characterClass));
        return this;
    }

    public ICharacterBuilder WithWeapon(Weapon weapon)
    {
        _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        return this;
    }

    public ICharacterBuilder WithArmor(Armor armor)
    {
        _armor = armor ?? throw new ArgumentNullException(nameof(armor));
        return this;
    }

    public ICharacterBuilder AddSkill(Skill skill)
    {
        _skills.Add(skill ?? throw new ArgumentNullException(nameof(skill)));
        return this;
    }

    public ICharacterBuilder AddSpecialAttribute(SpecialAttribute attribute)
    {
        _specialAttributes.Add(attribute ?? throw new ArgumentNullException(nameof(attribute)));
        return this;
    }

    public Character Build()
    {
        return new Character(
            _name ?? throw Missing("name"),
            _race ?? throw Missing("race"),
            _class ?? throw Missing("class"),
            _weapon,
            _armor,
            _skills,
            _specialAttributes);
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }

        return value.Trim();
    }

    private static InvalidOperationException Missing(string displayName)
        => new($"Cannot build a character because the {displayName} has not been configured.");
}
