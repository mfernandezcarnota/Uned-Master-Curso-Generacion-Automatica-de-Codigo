using TaxCalculationSystem.Domain;

namespace TaxCalculationSystem.TaxRules;

public sealed class GermanyTaxRule : ProportionalTaxRule
{
    protected override string CountryCode => "DE";

    protected override string CountryName => "Alemania";

    protected override decimal TaxRate => 0.19m;
}
