using RpgCharacterBuilder.Building;
using RpgCharacterBuilder.Domain;

namespace RpgCharacterBuilder.Presets;

public interface ICharacterPreset
{
    Character Create(string name, ICharacterBuilder builder);
}
