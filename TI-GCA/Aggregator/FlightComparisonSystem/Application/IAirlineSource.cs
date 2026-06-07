using FlightComparisonSystem.Domain;

namespace FlightComparisonSystem.Application;

public interface IAirlineSource
{
    string Name { get; }

    Task<IReadOnlyList<FlightOption>> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken = default);
}
