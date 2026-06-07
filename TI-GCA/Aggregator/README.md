# FlightComparisonSystem

Solución de consola en C# y .NET 8 que consulta varias compañías aéreas simultáneamente y devuelve una comparación unificada de vuelos.

## Funcionalidad

- Incluye inicialmente **Airline A**, **Airline B** y **Airline C**.
- Ejecuta todas las consultas de una búsqueda de forma concurrente.
- Consolida los vuelos disponibles y los ordena por precio y hora de salida.
- Conserva los resultados correctos aunque una o varias compañías fallen e informa del detalle de cada fallo.
- Permite incorporar nuevas compañías implementando `IAirlineSource` e incluyéndolas al construir `FlightComparisonService`.
- Propaga la cancelación solicitada por el consumidor.

## Diseño

La solución separa las responsabilidades en tres áreas:

- **Domain** contiene la solicitud de búsqueda, las opciones de vuelo y la respuesta consolidada.
- **Application** define el contrato común de las fuentes y coordina las consultas, la consolidación y los fallos parciales.
- **Infrastructure** contiene las fuentes iniciales y su configuración predeterminada.

`FlightComparisonService` solo depende de `IAirlineSource`. Por ello, una integración nueva puede añadirse sin modificar la lógica de comparación. Una respuesta consolidada distingue entre compañías consultadas correctamente y compañías no disponibles; una consulta correcta sin vuelos sigue considerándose exitosa.

## Estructura

```text
Aggregator/
├── FlightComparisonSystem.sln
├── FlightComparisonSystem/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/Airlines/
│   └── Program.cs
├── FlightComparisonSystem.Tests/
│   └── FlightComparisonServiceTests.cs
└── README.md
```

## Añadir una compañía

1. Crear una clase que implemente `IAirlineSource`.
2. Traducir la respuesta externa a una colección de `FlightOption`.
3. Registrar una instancia al construir `FlightComparisonService`.

```csharp
IAirlineSource nuevaCompania = new MyAirlineSource();
var service = new FlightComparisonService(
    DefaultAirlineSources.Create().Append(nuevaCompania));
```

## Ejecución

Desde este directorio:

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project FlightComparisonSystem --no-build
dotnet test --no-restore
```

Las pruebas cubren consulta individual, consulta múltiple concurrente, consolidación y ordenación de resultados, incorporación de una compañía nueva y gestión de fallos parciales.
