using LogisticsSystem.Application;
using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Infrastructure.VehicleCreators;

public sealed class MotorcycleCreator : VehicleCreator
{
    public override IDeliveryVehicle CreateVehicle() => new Motorcycle();
}
