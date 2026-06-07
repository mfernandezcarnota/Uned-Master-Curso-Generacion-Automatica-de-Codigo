namespace RpgCharacterBuilder.Domain;

public sealed record Weapon(string Name, int AttackPower)
{
    public Weapon(string name) : this(name, 0)
    {
    }
}

public sealed record Armor(string Name, int DefensePower)
{
    public Armor(string name) : this(name, 0)
    {
    }
}
