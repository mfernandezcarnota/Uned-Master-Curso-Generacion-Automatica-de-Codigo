using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Application;

public sealed record DispatchResult(IDeliveryVehicle Vehicle, string Description);
