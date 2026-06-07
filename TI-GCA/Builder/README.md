# RpgCharacterBuilder

Solución .NET 8 para crear personajes de RPG mediante una API fluida, legible y ampliable. Permite configurar raza, clase, arma, armadura, habilidades y atributos especiales, tanto de forma manual como a partir de configuraciones reutilizables.

## Proyectos

- **RpgCharacterBuilder**: biblioteca principal con el modelo de dominio, el proceso de construcción y configuraciones predefinidas.
- **RpgCharacterBuilder.Tests**: pruebas unitarias xUnit para los escenarios principales y los puntos de extensión.

## Arquitectura

La solución separa tres responsabilidades:

1. **Representación (`Domain`)**: `Character` es el resultado final y expone colecciones de solo lectura. Los conceptos del juego se modelan con tipos específicos (`Race`, `CharacterClass`, `Weapon`, `Armor`, `Skill` y `SpecialAttribute`) para evitar parámetros ambiguos.
2. **Construcción (`Building`)**: `ICharacterBuilder` define las operaciones disponibles y `RpgCharacterBuilder` conserva el estado temporal, valida los datos obligatorios y crea la representación final únicamente al invocar `Build()`.
3. **Configuraciones reutilizables (`Presets`)**: las clases que implementan `ICharacterPreset` describen combinaciones habituales sin acoplarlas a la representación interna del personaje.

Esta división permite modificar la forma de construir personajes o añadir configuraciones sin alterar `Character`.

## Estructura de clases

```text
RpgCharacterBuilder/
├── Building/
│   ├── ICharacterBuilder.cs
│   └── RpgCharacterBuilder.cs
├── Domain/
│   ├── Character.cs
│   ├── CharacterClass.cs
│   ├── Equipment.cs
│   ├── NamedValue.cs
│   ├── Race.cs
│   ├── Skill.cs
│   └── SpecialAttribute.cs
└── Presets/
    ├── ElfMagePreset.cs
    ├── HumanWarriorPreset.cs
    └── ICharacterPreset.cs
```

## Decisiones de diseño

- La API devuelve `ICharacterBuilder` en cada paso para encadenar operaciones o ejecutarlas una a una.
- Nombre, raza y clase son obligatorios. Arma, armadura, habilidades y atributos especiales son opcionales.
- `Character` copia las colecciones temporales y las expone como solo lectura, evitando que cambios posteriores alteren un personaje ya creado.
- `Race` y `CharacterClass` son valores nominales ampliables en lugar de enumeraciones cerradas. `Custom(...)` permite incorporar contenido nuevo sin modificar el código existente.
- Las configuraciones reutilizables dependen de `ICharacterBuilder`, lo que permite sustituir la implementación de construcción.
- Cada responsabilidad vive en un archivo independiente y el proyecto tiene referencias unidireccionales claras.

## Uso

```csharp
using RpgCharacterBuilder.Building;
using RpgCharacterBuilder.Domain;
using CharacterBuilder = RpgCharacterBuilder.Building.RpgCharacterBuilder;

var ranger = new CharacterBuilder()
    .Named("Nim")
    .OfRace(Race.Elf)
    .WithClass(CharacterClass.Custom("Ranger"))
    .WithWeapon(new Weapon("Longbow", 15))
    .AddSkill(new Skill("Forest Camouflage"))
    .AddSpecialAttribute(new SpecialAttribute("Perception", "+4"))
    .Build();
```

También puede aplicarse una configuración reutilizable:

```csharp
using RpgCharacterBuilder.Presets;

var warrior = new HumanWarriorPreset().Create("Aldric", new CharacterBuilder());
```

## Compilación y pruebas

Desde este directorio:

```bash
dotnet restore RpgCharacterBuilder.sln
dotnet build RpgCharacterBuilder.sln --no-restore
dotnet test RpgCharacterBuilder.sln --no-build
```
