using TaxCalculationSystem.Application;
using TaxCalculationSystem.Domain;
using TaxCalculationSystem.TaxRules;

var calculator = new TaxCalculator(new SpainTaxRule());
Print(calculator.Calculate(1_000m));

calculator.ChangeRule(new GermanyTaxRule());
Print(calculator.Calculate(1_000m));

static void Print(TaxCalculation calculation)
{
    Console.WriteLine(
        $"{calculation.CountryName}: base {calculation.TaxableAmount:C}, " +
        $"impuesto {calculation.TaxAmount:C}, total {calculation.TotalAmount:C}");
}
