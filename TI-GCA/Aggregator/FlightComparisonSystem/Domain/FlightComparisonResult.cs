namespace FlightComparisonSystem.Domain;

public sealed record FlightComparisonResult(
    IReadOnlyList<FlightOption> Flights,
    IReadOnlyList<string> SuccessfulAirlines,
    IReadOnlyList<AirlineFailure> Failures)
{
    public bool IsPartial => Failures.Count > 0;
}
