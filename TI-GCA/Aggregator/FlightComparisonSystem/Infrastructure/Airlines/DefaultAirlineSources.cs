using FlightComparisonSystem.Application;

namespace FlightComparisonSystem.Infrastructure.Airlines;

public static class DefaultAirlineSources
{
    public static IReadOnlyList<IAirlineSource> Create() =>
    [
        new ScheduledAirlineSource("Airline A", "AA", new TimeOnly(8, 0), TimeSpan.FromHours(2), 180m),
        new ScheduledAirlineSource("Airline B", "BB", new TimeOnly(10, 30), TimeSpan.FromMinutes(135), 145m),
        new ScheduledAirlineSource("Airline C", "CC", new TimeOnly(16, 15), TimeSpan.FromMinutes(125), 210m)
    ];
}
