using TaxCalculationSystem.Domain;

namespace TaxCalculationSystem.Application;

public sealed class TaxCalculator
{
    private ITaxRule _taxRule;

    public TaxCalculator(ITaxRule taxRule)
    {
        _taxRule = taxRule ?? throw new ArgumentNullException(nameof(taxRule));
    }

    public void ChangeRule(ITaxRule taxRule)
    {
        _taxRule = taxRule ?? throw new ArgumentNullException(nameof(taxRule));
    }

    public TaxCalculation Calculate(decimal taxableAmount) => _taxRule.Calculate(taxableAmount);
}
