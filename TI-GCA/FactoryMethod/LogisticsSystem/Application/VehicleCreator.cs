using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Application;

public abstract class VehicleCreator
{
    public abstract IDeliveryVehicle CreateVehicle();
}
