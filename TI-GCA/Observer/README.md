# StockMonitoringSystem

Solución .NET 8 para monitorizar acciones y comunicar automáticamente sus cambios de precio a múltiples inversores registrados.

## Proyectos

- **StockMonitoringSystem**: biblioteca principal con los contratos, el modelo de actualización, las acciones monitorizadas y los inversores.
- **StockMonitoringSystem.Tests**: pruebas unitarias xUnit que verifican el registro, el registro múltiple, la eliminación y las notificaciones ante cambios consecutivos.

## Arquitectura

La solución distribuye sus responsabilidades entre cuatro áreas:

1. **Contratos (`Abstractions`)**: `IStockPricePublisher` define el registro y la eliminación de destinatarios; `IStockPriceObserver` define cómo recibe un destinatario una actualización. El código principal depende de estas abstracciones y admite nuevas implementaciones sin modificar las existentes.
2. **Dominio (`Domain`)**: `StockPriceUpdate` representa un cambio inmutable con símbolo, precio anterior, precio actual y momento de actualización.
3. **Monitorización (`Monitoring`)**: `Stock` conserva el precio vigente, administra los destinatarios registrados y comunica cada cambio real. Una copia de los destinatarios garantiza que la entrega permanezca estable aunque se modifique un registro durante una notificación.
4. **Inversores (`Investors`)**: `Investor` recibe y conserva su historial de notificaciones como una colección de solo lectura.

Cada clase tiene una responsabilidad concreta. La separación mediante interfaces permite incorporar destinatarios alternativos, como alertas por correo o paneles en tiempo real, sin acoplarlos a `Stock`.

## Comportamiento

- Una acción puede registrar uno o varios inversores.
- El mismo inversor no se registra dos veces en una acción.
- Un inversor eliminado deja de recibir cambios posteriores.
- Solo un precio diferente al vigente genera una notificación.
- Cada actualización contiene el precio anterior y el nuevo, por lo que las actualizaciones consecutivas mantienen su secuencia.
- Los símbolos se normalizan a mayúsculas y los precios negativos se rechazan.

## Uso

```csharp
using StockMonitoringSystem.Investors;
using StockMonitoringSystem.Monitoring;

var stock = new Stock("ACME", 100m);
var ana = new Investor("Ana");
var bruno = new Investor("Bruno");

stock.Register(ana);
stock.Register(bruno);
stock.UpdatePrice(105.50m);
stock.Remove(bruno);
stock.UpdatePrice(103m);

Console.WriteLine(ana.Notifications.Count);    // 2
Console.WriteLine(bruno.Notifications.Count); // 1
```

## Estructura

```text
Observer/
├── StockMonitoringSystem.sln
├── StockMonitoringSystem/
│   ├── Abstractions/
│   ├── Domain/
│   ├── Investors/
│   └── Monitoring/
└── StockMonitoringSystem.Tests/
```

## Compilación y pruebas

Desde este directorio:

```bash
dotnet restore StockMonitoringSystem.sln
dotnet build StockMonitoringSystem.sln --no-restore
dotnet test StockMonitoringSystem.sln --no-build
```
