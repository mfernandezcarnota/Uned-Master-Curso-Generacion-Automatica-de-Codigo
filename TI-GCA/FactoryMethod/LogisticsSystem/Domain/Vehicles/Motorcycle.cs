namespace LogisticsSystem.Domain.Vehicles;

public sealed class Motorcycle : DeliveryVehicle
{
    public Motorcycle() : base("Motocicleta", 30)
    {
    }

    protected override string TransportDescription => "con entrega rápida de última milla";
}
