using RpgCharacterBuilder.Building;
using RpgCharacterBuilder.Domain;
using RpgCharacterBuilder.Presets;
using CharacterBuilder = RpgCharacterBuilder.Building.RpgCharacterBuilder;

namespace RpgCharacterBuilder.Tests;

public sealed class CharacterCreationTests
{
    [Fact]
    public void CreatesHumanWarriorFromReusableConfiguration()
    {
        var warrior = new HumanWarriorPreset().Create("Aldric", new CharacterBuilder());

        Assert.Equal("Aldric", warrior.Name);
        Assert.Equal(Race.Human, warrior.Race);
        Assert.Equal(CharacterClass.Warrior, warrior.Class);
        Assert.Equal("Longsword", warrior.Weapon?.Name);
        Assert.Equal("Plate armor", warrior.Armor?.Name);
        Assert.Contains(warrior.Skills, skill => skill.Name == "Shield Bash");
    }

    [Fact]
    public void CreatesElfMageFromReusableConfiguration()
    {
        var mage = new ElfMagePreset().Create("Lyriel", new CharacterBuilder());

        Assert.Equal(Race.Elf, mage.Race);
        Assert.Equal(CharacterClass.Mage, mage.Class);
        Assert.Equal("Oak staff", mage.Weapon?.Name);
        Assert.Contains(mage.Skills, skill => skill.Name == "Arcane Missile");
        Assert.Contains(mage.SpecialAttributes, attribute => attribute.Name == "Intelligence");
    }

    [Fact]
    public void CreatesCustomizedArcher()
    {
        var archer = new CharacterBuilder()
            .Named("Robin")
            .OfRace(Race.Human)
            .WithClass(CharacterClass.Archer)
            .WithWeapon(new Weapon("Yew longbow", 16))
            .WithArmor(new Armor("Leather armor", 8))
            .AddSkill(new Skill("Multi-shot"))
            .AddSpecialAttribute(new SpecialAttribute("Accuracy", "+5"))
            .Build();

        Assert.Equal(CharacterClass.Archer, archer.Class);
        Assert.Equal("Yew longbow", archer.Weapon?.Name);
        Assert.Single(archer.Skills);
        Assert.Equal("+5", Assert.Single(archer.SpecialAttributes).Value);
    }

    [Fact]
    public void AcceptsANewCharacterClassWithoutChangingConstructionCode()
    {
        var bard = new CharacterBuilder()
            .Named("Melody")
            .OfRace(Race.Elf)
            .WithClass(CharacterClass.Custom("Bard"))
            .WithWeapon(new Weapon("Lute", 2))
            .AddSkill(new Skill("Song of Courage"))
            .Build();

        Assert.Equal("Bard", bard.Class.Name);
        Assert.Equal("Song of Courage", Assert.Single(bard.Skills).Name);
    }

    [Fact]
    public void ConstructsCharacterStepByStepAndRequiresCoreProperties()
    {
        ICharacterBuilder builder = new CharacterBuilder();

        var incompleteError = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("name", incompleteError.Message);

        builder.Named("Brakka");
        builder.OfRace(Race.Orc);
        builder.WithClass(CharacterClass.Warrior);
        builder.WithWeapon(new Weapon("War axe", 19));
        builder.AddSkill(new Skill("Charge"));

        var character = builder.Build();

        Assert.Equal("Brakka", character.Name);
        Assert.Equal(Race.Orc, character.Race);
        Assert.Null(character.Armor);
        Assert.Equal("Charge", Assert.Single(character.Skills).Name);
    }
}
