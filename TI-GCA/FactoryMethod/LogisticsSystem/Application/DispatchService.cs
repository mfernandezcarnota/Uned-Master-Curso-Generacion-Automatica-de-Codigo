namespace LogisticsSystem.Application;

public sealed class DispatchService(VehicleCreator vehicleCreator)
{
    private readonly VehicleCreator _vehicleCreator = vehicleCreator ?? throw new ArgumentNullException(nameof(vehicleCreator));

    public DispatchResult Dispatch(string destination)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);

        var vehicle = _vehicleCreator.CreateVehicle();
        var description = vehicle.DeliverTo(destination);

        return new DispatchResult(vehicle, description);
    }
}
