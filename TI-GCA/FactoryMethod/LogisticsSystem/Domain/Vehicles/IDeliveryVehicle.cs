namespace LogisticsSystem.Domain.Vehicles;

public interface IDeliveryVehicle
{
    string Name { get; }

    int MaximumLoadInKilograms { get; }

    string DeliverTo(string destination);
}
