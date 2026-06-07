using LogisticsSystem.Application;
using LogisticsSystem.Infrastructure.VehicleCreators;

var creators = new VehicleCreator[]
{
    new TruckCreator(),
    new VanCreator(),
    new MotorcycleCreator()
};

foreach (var creator in creators)
{
    var dispatch = new DispatchService(creator).Dispatch("Centro logístico");
    Console.WriteLine(dispatch.Description);
}
