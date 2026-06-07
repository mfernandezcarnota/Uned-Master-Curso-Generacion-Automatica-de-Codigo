using RpgCharacterBuilder.Domain;

namespace RpgCharacterBuilder.Building;

public interface ICharacterBuilder
{
    ICharacterBuilder Named(string name);

    ICharacterBuilder OfRace(Race race);

    ICharacterBuilder WithClass(CharacterClass characterClass);

    ICharacterBuilder WithWeapon(Weapon weapon);

    ICharacterBuilder WithArmor(Armor armor);

    ICharacterBuilder AddSkill(Skill skill);

    ICharacterBuilder AddSpecialAttribute(SpecialAttribute attribute);

    Character Build();
}
