# TaxCalculationSystem

Solución de consola en C# y .NET 8 para calcular impuestos internacionales aplicando la normativa fiscal seleccionada durante la ejecución.

## Funcionalidad

- Incluye reglas fiscales iniciales para España (21 %) y Alemania (19 %).
- Permite cambiar la regla activa en cualquier momento mediante `ChangeRule`.
- Permite incorporar países sin modificar `TaxCalculator`.
- Valida que la base imponible no sea negativa.
- Redondea los impuestos a dos decimales mediante redondeo comercial.

Los porcentajes se utilizan como valores sencillos de demostración y no pretenden representar asesoramiento fiscal.

## Arquitectura

La solución separa las responsabilidades en tres áreas:

- **Dominio**: define `ITaxRule`, el resultado inmutable `TaxCalculation` y la base reutilizable `ProportionalTaxRule`.
- **Aplicación**: `TaxCalculator` coordina el cálculo utilizando únicamente el contrato del dominio y permite sustituir la regla activa.
- **Reglas fiscales**: cada país encapsula de forma independiente sus datos y su cálculo.

Las dependencias apuntan hacia abstracciones pequeñas. Para añadir una normativa con cálculo proporcional se puede heredar de `ProportionalTaxRule`; para una normativa con un cálculo diferente basta con implementar `ITaxRule`. Ninguna de las dos opciones requiere cambiar el calculador existente.

## Estructura

```text
Strategy/
├── TaxCalculationSystem.sln
├── TaxCalculationSystem/
│   ├── Application/TaxCalculator.cs
│   ├── Domain/
│   │   ├── ITaxRule.cs
│   │   ├── ProportionalTaxRule.cs
│   │   └── TaxCalculation.cs
│   ├── TaxRules/
│   │   ├── GermanyTaxRule.cs
│   │   └── SpainTaxRule.cs
│   └── Program.cs
├── TaxCalculationSystem.Tests/
│   ├── TaxCalculatorTests.cs
│   └── ValidationTests.cs
└── README.md
```

## Incorporar un país

Una regla proporcional nueva solo necesita indicar su código, nombre y tipo impositivo:

```csharp
public sealed class FranceTaxRule : ProportionalTaxRule
{
    protected override string CountryCode => "FR";
    protected override string CountryName => "Francia";
    protected override decimal TaxRate => 0.20m;
}
```

Después puede proporcionarse al calculador al crearlo o durante la ejecución:

```csharp
var calculator = new TaxCalculator(new FranceTaxRule());
calculator.ChangeRule(new SpainTaxRule());
```

## Ejecución

Desde este directorio:

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project TaxCalculationSystem
dotnet test --no-restore
```

Las pruebas cubren los cálculos de España y Alemania, el cambio dinámico de reglas, la incorporación de Francia sin modificar el proyecto principal y las validaciones básicas.
