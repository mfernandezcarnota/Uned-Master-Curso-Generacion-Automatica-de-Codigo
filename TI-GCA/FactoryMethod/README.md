# LogisticsSystem

Aplicación de consola en C# y .NET 8 para crear vehículos de reparto y coordinar entregas sin acoplar la lógica de negocio a tipos concretos.

## Arquitectura

La solución separa responsabilidades en tres áreas:

- **Dominio**: contiene el contrato `IDeliveryVehicle` y los vehículos de reparto. Cada vehículo encapsula sus datos y su comportamiento de entrega.
- **Aplicación**: contiene `DispatchService`, el caso de uso que coordina un envío, y `VehicleCreator`, la abstracción mediante la que obtiene un vehículo.
- **Infraestructura**: contiene las implementaciones que crean los vehículos concretos disponibles.

La dirección de las dependencias mantiene la lógica de aplicación aislada de los vehículos concretos. `DispatchService` solo conoce abstracciones, por lo que puede probarse de forma independiente y admite nuevas implementaciones sin cambios internos.

## Estructura

```text
FactoryMethod/
├── LogisticsSystem.sln
├── LogisticsSystem/
│   ├── Application/                 # Coordinación de casos de uso y contratos de creación
│   ├── Domain/Vehicles/             # Contratos y entidades del dominio
│   ├── Infrastructure/VehicleCreators/ # Creadores de vehículos concretos
│   └── Program.cs                   # Punto de entrada y composición
├── LogisticsSystem.Tests/
│   ├── DispatchServiceTests.cs
│   ├── ExtensibilityTests.cs
│   └── VehicleCreationTests.cs
└── README.md
```

## Decisiones de diseño

- Cada clase tiene una responsabilidad clara y se organiza en su propio archivo.
- Los vehículos implementan un contrato pequeño y enfocado.
- La creación de objetos concretos queda fuera del servicio que contiene la lógica de negocio.
- Añadir un vehículo requiere implementar `IDeliveryVehicle` y proporcionar un nuevo `VehicleCreator`; no requiere modificar los servicios existentes.
- Las dependencias se reciben desde fuera para facilitar sustituciones y pruebas unitarias.
- Se habilitan tipos anulables y *implicit usings* de .NET 8.

## Ejecución

Desde este directorio:

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project LogisticsSystem
dotnet test --no-restore
```
