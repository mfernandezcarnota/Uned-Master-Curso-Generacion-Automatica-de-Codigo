using WarehouseRoutingSystem.Domain;

namespace WarehouseRoutingSystem.Application;

public static class OrderRoutingSystemFactory
{
    public static OrderRouter CreateDefault()
    {
        var router = new OrderRouter();

        router.RegisterCategory(InitialCategories.Refrigerated);
        router.RegisterCategory(InitialCategories.Fragile);
        router.RegisterCategory(InitialCategories.Standard);

        router.RegisterRule(new ClassificationRule(
            "Necesita cadena de frío",
            InitialCategories.Refrigerated,
            priority: 200,
            order => order.RequiresRefrigeration));
        router.RegisterRule(new ClassificationRule(
            "Requiere manipulación delicada",
            InitialCategories.Fragile,
            priority: 100,
            order => order.IsFragile));
        router.RegisterRule(new ClassificationRule(
            "Clasificación general",
            InitialCategories.Standard,
            priority: int.MinValue,
            _ => true));

        return router;
    }
}
