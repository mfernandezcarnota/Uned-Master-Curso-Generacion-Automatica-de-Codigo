namespace LogisticsSystem.Domain.Vehicles;

public sealed class Truck : DeliveryVehicle
{
    public Truck() : base("Camión", 12_000)
    {
    }

    protected override string TransportDescription => "por carretera con carga pesada";
}
