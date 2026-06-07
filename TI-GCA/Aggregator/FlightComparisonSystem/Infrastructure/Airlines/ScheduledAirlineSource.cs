using FlightComparisonSystem.Application;
using FlightComparisonSystem.Domain;

namespace FlightComparisonSystem.Infrastructure.Airlines;

public sealed class ScheduledAirlineSource(
    string name,
    string flightPrefix,
    TimeOnly departureTime,
    TimeSpan duration,
    decimal price) : IAirlineSource
{
    public string Name { get; } = name;

    public async Task<IReadOnlyList<FlightOption>> SearchAsync(
        FlightSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(25), cancellationToken);

        var departure = new DateTimeOffset(
            request.DepartureDate.ToDateTime(departureTime),
            TimeSpan.Zero);

        return
        [
            new FlightOption(
                Name,
                $"{flightPrefix}101",
                request.Origin,
                request.Destination,
                departure,
                departure.Add(duration),
                price,
                "EUR")
        ];
    }
}
