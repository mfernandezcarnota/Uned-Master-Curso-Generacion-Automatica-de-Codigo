namespace LogisticsSystem.Domain.Vehicles;

public sealed class Van : DeliveryVehicle
{
    public Van() : base("Furgoneta", 1_500)
    {
    }

    protected override string TransportDescription => "por carretera en ruta urbana";
}
