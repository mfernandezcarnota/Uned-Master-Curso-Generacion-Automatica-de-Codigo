using RpgCharacterBuilder.Building;
using RpgCharacterBuilder.Domain;

namespace RpgCharacterBuilder.Presets;

public sealed class ElfMagePreset : ICharacterPreset
{
    public Character Create(string name, ICharacterBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder
            .Named(name)
            .OfRace(Race.Elf)
            .WithClass(CharacterClass.Mage)
            .WithWeapon(new Weapon("Oak staff", 9))
            .WithArmor(new Armor("Enchanted robe", 6))
            .AddSkill(new Skill("Arcane Missile", "Launches a focused magic projectile."))
            .AddSkill(new Skill("Mana Shield", "Transforms mana into temporary protection."))
            .AddSpecialAttribute(new SpecialAttribute("Intelligence", "+4"))
            .Build();
    }
}
