using LogisticsSystem.Application;
using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Tests;

public sealed class ExtensibilityTests
{
    [Fact]
    public void NewVehicleType_CanBeAddedWithoutChangingExistingCode()
    {
        var service = new DispatchService(new BicycleCreator());

        var result = service.Dispatch("Calle Mayor 1");

        Assert.IsType<Bicycle>(result.Vehicle);
        Assert.Contains("bicicleta", result.Description, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class BicycleCreator : VehicleCreator
    {
        public override IDeliveryVehicle CreateVehicle() => new Bicycle();
    }

    private sealed class Bicycle : IDeliveryVehicle
    {
        public string Name => "Bicicleta";

        public int MaximumLoadInKilograms => 10;

        public string DeliverTo(string destination) => $"Bicicleta realiza la entrega en {destination}.";
    }
}
