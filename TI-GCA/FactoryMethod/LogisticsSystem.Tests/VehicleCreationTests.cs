using LogisticsSystem.Domain.Vehicles;
using LogisticsSystem.Infrastructure.VehicleCreators;

namespace LogisticsSystem.Tests;

public sealed class VehicleCreationTests
{
    [Fact]
    public void TruckCreator_CreatesTruck()
    {
        var vehicle = new TruckCreator().CreateVehicle();

        var truck = Assert.IsType<Truck>(vehicle);
        Assert.Equal("Camión", truck.Name);
        Assert.Equal(12_000, truck.MaximumLoadInKilograms);
    }

    [Fact]
    public void VanCreator_CreatesVan()
    {
        var vehicle = new VanCreator().CreateVehicle();

        var van = Assert.IsType<Van>(vehicle);
        Assert.Equal("Furgoneta", van.Name);
        Assert.Equal(1_500, van.MaximumLoadInKilograms);
    }

    [Fact]
    public void MotorcycleCreator_CreatesMotorcycle()
    {
        var vehicle = new MotorcycleCreator().CreateVehicle();

        var motorcycle = Assert.IsType<Motorcycle>(vehicle);
        Assert.Equal("Motocicleta", motorcycle.Name);
        Assert.Equal(30, motorcycle.MaximumLoadInKilograms);
    }
}
