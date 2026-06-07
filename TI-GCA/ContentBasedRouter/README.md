# WarehouseRoutingSystem

Solución de consola en C# y .NET 8 que clasifica pedidos según sus características y los dirige automáticamente al almacén adecuado.

## Capacidades

- Clasificación inicial de pedidos **Refrigerados**, **Frágiles** y **Estándar**.
- Reglas priorizadas: cuando un pedido cumple varias condiciones, se aplica primero la regla con mayor prioridad.
- Registro de categorías y reglas adicionales sin modificar el servicio de enrutamiento.
- Procesamiento seguro de múltiples pedidos mediante una operación asíncrona concurrente.
- Conservación del orden de entrada en los resultados de un lote.

## Estructura

```text
./
├── WarehouseRoutingSystem.sln
├── WarehouseRoutingSystem/
│   ├── Application/                 # Clasificación, reglas y decisiones de envío
│   ├── Domain/                      # Pedidos y categorías
│   └── Program.cs                   # Ejemplo de ejecución
├── WarehouseRoutingSystem.Tests/
│   └── OrderRouterTests.cs          # Pruebas de categorías, extensión y concurrencia
└── README.md
```

## Diseño

`OrderRouter` mantiene un catálogo de categorías y una lista priorizada de reglas. Cada regla contiene una condición aplicable a un pedido y la categoría de destino. Las reglas se publican como una instantánea inmutable, por lo que las lecturas concurrentes no bloquean y siempre observan una colección consistente.

La categoría estándar usa una regla final de respaldo. Para incorporar una categoría nueva solo es necesario registrarla y añadir su regla:

```csharp
var hazardous = new OrderCategory("Peligroso", "ALM-PEL");
router.RegisterCategory(hazardous);
router.RegisterRule(new ClassificationRule(
    "Contiene materiales peligrosos",
    hazardous,
    priority: 300,
    order => order.HasAttribute("Peligroso", "Sí")));
```

## Ejecución

Desde este directorio:

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project WarehouseRoutingSystem
dotnet test --no-restore
```

## Pruebas incluidas

- Pedido refrigerado.
- Pedido frágil.
- Pedido estándar.
- Registro y uso de una categoría nueva.
- Clasificación concurrente de un lote de pedidos.
