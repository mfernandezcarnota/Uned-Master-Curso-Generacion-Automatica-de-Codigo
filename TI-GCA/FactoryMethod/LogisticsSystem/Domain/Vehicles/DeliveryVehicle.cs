namespace LogisticsSystem.Domain.Vehicles;

public abstract class DeliveryVehicle(string name, int maximumLoadInKilograms) : IDeliveryVehicle
{
    public string Name { get; } = name;

    public int MaximumLoadInKilograms { get; } = maximumLoadInKilograms;

    protected abstract string TransportDescription { get; }

    public string DeliverTo(string destination)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destination);
        return $"{Name} realiza la entrega en {destination} {TransportDescription}.";
    }
}
