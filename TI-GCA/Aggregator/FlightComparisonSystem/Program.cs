using FlightComparisonSystem.Application;
using FlightComparisonSystem.Domain;
using FlightComparisonSystem.Infrastructure.Airlines;

var service = new FlightComparisonService(DefaultAirlineSources.Create());
var request = new FlightSearchRequest("MAD", "LHR", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)));
var result = await service.SearchAsync(request);

Console.WriteLine($"Flights from {request.Origin} to {request.Destination} on {request.DepartureDate:yyyy-MM-dd}:");
foreach (var flight in result.Flights)
{
    Console.WriteLine($"- {flight.Airline} {flight.FlightNumber}: {flight.Price:0.00} {flight.Currency}");
}

if (result.IsPartial)
{
    Console.WriteLine($"Unavailable airlines: {string.Join(", ", result.Failures.Select(failure => failure.Airline))}");
}
