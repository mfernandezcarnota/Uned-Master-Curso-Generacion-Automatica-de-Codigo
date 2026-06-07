using RpgCharacterBuilder.Building;
using RpgCharacterBuilder.Domain;

namespace RpgCharacterBuilder.Presets;

public sealed class HumanWarriorPreset : ICharacterPreset
{
    public Character Create(string name, ICharacterBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder
            .Named(name)
            .OfRace(Race.Human)
            .WithClass(CharacterClass.Warrior)
            .WithWeapon(new Weapon("Longsword", 18))
            .WithArmor(new Armor("Plate armor", 20))
            .AddSkill(new Skill("Shield Bash", "Stuns a close enemy."))
            .AddSkill(new Skill("Battle Cry", "Raises nearby allies' morale."))
            .AddSpecialAttribute(new SpecialAttribute("Constitution", "+3"))
            .Build();
    }
}
