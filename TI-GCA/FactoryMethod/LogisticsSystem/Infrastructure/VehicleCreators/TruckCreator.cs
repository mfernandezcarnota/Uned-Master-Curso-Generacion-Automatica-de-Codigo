using LogisticsSystem.Application;
using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Infrastructure.VehicleCreators;

public sealed class TruckCreator : VehicleCreator
{
    public override IDeliveryVehicle CreateVehicle() => new Truck();
}
