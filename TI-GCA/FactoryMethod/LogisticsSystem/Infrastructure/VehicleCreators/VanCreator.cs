using LogisticsSystem.Application;
using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Infrastructure.VehicleCreators;

public sealed class VanCreator : VehicleCreator
{
    public override IDeliveryVehicle CreateVehicle() => new Van();
}
