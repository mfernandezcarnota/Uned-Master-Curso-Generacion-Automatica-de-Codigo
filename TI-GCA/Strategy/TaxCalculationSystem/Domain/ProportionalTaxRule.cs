namespace TaxCalculationSystem.Domain;

public abstract class ProportionalTaxRule : ITaxRule
{
    protected abstract string CountryCode { get; }

    protected abstract string CountryName { get; }

    protected abstract decimal TaxRate { get; }

    public TaxCalculation Calculate(decimal taxableAmount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(taxableAmount);

        var taxAmount = decimal.Round(
            taxableAmount * TaxRate,
            2,
            MidpointRounding.AwayFromZero);

        return new TaxCalculation(CountryCode, CountryName, taxableAmount, taxAmount);
    }
}
