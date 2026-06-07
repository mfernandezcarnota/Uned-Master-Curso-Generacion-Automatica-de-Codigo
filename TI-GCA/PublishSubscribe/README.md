# OrderEventSystem

Solución en C# y .NET 8 que modela la creación de pedidos de una plataforma de comercio electrónico y comunica cada nuevo pedido a los servicios interesados sin acoplarlos al caso de uso principal.

## Funcionamiento

`OrderService` crea y almacena el pedido y, a continuación, emite un `OrderCreatedEvent` mediante el contrato `IEventBus`. Los consumidores se registran desde el punto de composición y reciben el evento a través de `IEventConsumer<TEvent>`.

La aplicación incluye cuatro consumidores iniciales:

- `InventoryService`
- `BillingService`
- `LogisticsService`
- `NotificationService`

`InMemoryEventBus` permite registrar y retirar consumidores durante la ejecución. Al emitir un evento toma una instantánea de los consumidores registrados y los ejecuta de forma asíncrona. De este modo, incorporar un servicio nuevo solo requiere implementar el contrato del consumidor y registrarlo; `OrderService` no necesita cambios.

## Estructura

```text
OrderEventSystem.sln
├── OrderEventSystem/
│   ├── Application/       # Caso de uso de pedidos y contrato del repositorio
│   ├── Domain/            # Pedido, artículos y evento de creación
│   ├── Infrastructure/    # Repositorio en memoria
│   ├── Messaging/         # Contratos y distribuidor de eventos en memoria
│   ├── Services/          # Consumidores iniciales
│   └── Program.cs         # Ejemplo de composición y ejecución
└── OrderEventSystem.Tests/
    └── OrderEventSystemTests.cs
```

## Decisiones de diseño

- El caso de uso depende únicamente de `IOrderRepository` e `IEventBus`.
- Los consumidores dependen del contrato genérico `IEventConsumer<TEvent>` y no conocen al creador de pedidos.
- Los registros y las bajas de consumidores están protegidos para permitir acceso concurrente.
- La distribución usa una instantánea para que los cambios de registro no alteren una entrega que ya está en curso.
- Registrar dos veces la misma instancia no provoca entregas duplicadas.
- Las operaciones asíncronas aceptan `CancellationToken`.

## Ejecución

Desde este directorio:

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project OrderEventSystem --no-build
dotnet test --no-restore
```

## Pruebas incluidas

La suite automatizada comprueba:

1. La creación y persistencia de un pedido.
2. La recepción del evento por un consumidor registrado.
3. La distribución a todos los servicios iniciales.
4. La incorporación de un consumidor adicional sin modificar el caso de uso.
5. La retirada de un consumidor para que deje de recibir eventos futuros.
