using TaxCalculationSystem.Domain;

namespace TaxCalculationSystem.TaxRules;

public sealed class SpainTaxRule : ProportionalTaxRule
{
    protected override string CountryCode => "ES";

    protected override string CountryName => "España";

    protected override decimal TaxRate => 0.21m;
}
