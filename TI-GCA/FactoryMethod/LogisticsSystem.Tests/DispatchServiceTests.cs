using LogisticsSystem.Application;
using LogisticsSystem.Domain.Vehicles;

namespace LogisticsSystem.Tests;

public sealed class DispatchServiceTests
{
    [Fact]
    public void Dispatch_DependsOnAbstractionsAndDelegatesDeliveryToCreatedVehicle()
    {
        var vehicle = new TestVehicle();
        var service = new DispatchService(new TestVehicleCreator(vehicle));

        var result = service.Dispatch("Almacén norte");

        Assert.Same(vehicle, result.Vehicle);
        Assert.Equal("Almacén norte", vehicle.ReceivedDestination);
        Assert.Equal("Entrega de prueba completada.", result.Description);
    }

    private sealed class TestVehicleCreator(IDeliveryVehicle vehicle) : VehicleCreator
    {
        public override IDeliveryVehicle CreateVehicle() => vehicle;
    }

    private sealed class TestVehicle : IDeliveryVehicle
    {
        public string Name => "Vehículo de prueba";

        public int MaximumLoadInKilograms => 1;

        public string? ReceivedDestination { get; private set; }

        public string DeliverTo(string destination)
        {
            ReceivedDestination = destination;
            return "Entrega de prueba completada.";
        }
    }
}
