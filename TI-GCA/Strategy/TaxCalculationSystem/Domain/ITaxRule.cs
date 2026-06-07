namespace TaxCalculationSystem.Domain;

public interface ITaxRule
{
    TaxCalculation Calculate(decimal taxableAmount);
}
