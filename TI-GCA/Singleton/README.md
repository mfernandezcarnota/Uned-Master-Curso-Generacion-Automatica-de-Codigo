# Global Configuration System

Solución .NET 8 que ofrece un punto de acceso global, único y seguro para la configuración compartida de una aplicación empresarial.

## Arquitectura

La biblioteca separa las responsabilidades en tres piezas:

- `ConfigurationSnapshot` es un registro inmutable. Cada lectura obtiene una vista completa y coherente, sin observar cambios parciales.
- `IGlobalConfiguration` define el contrato mínimo de lectura y actualización. Los módulos dependen de esta abstracción y pueden recibir otra implementación durante pruebas aisladas.
- `GlobalConfiguration` controla el acceso global. Su construcción es privada, la inicialización es diferida y segura entre hilos, y las actualizaciones utilizan intercambio atómico con reintento para no perder cambios concurrentes.

Los módulos de ejemplo aceptan el contrato mediante inyección de dependencias, pero usan el acceso global como valor predeterminado. Así se conserva la facilidad de uso desde cualquier módulo sin acoplar su lógica a la implementación concreta.

## Estructura

```text
./
├── GlobalConfigurationSystem.sln
├── GlobalConfigurationSystem/
│   ├── Configuration/
│   │   ├── ConfigurationSnapshot.cs
│   │   ├── GlobalConfiguration.cs
│   │   └── IGlobalConfiguration.cs
│   ├── Modules/
│   │   ├── AuditConfigurationReader.cs
│   │   └── BillingConfigurationReader.cs
│   └── GlobalConfigurationSystem.csproj
├── GlobalConfigurationSystem.Tests/
│   ├── GlobalConfigurationTests.cs
│   └── GlobalConfigurationSystem.Tests.csproj
└── README.md
```

## Decisiones de diseño

- **Instancia única y acceso controlado:** el constructor no forma parte de la API pública; el único acceso disponible devuelve siempre el mismo objeto.
- **Seguridad multihilo:** la creación se publica una sola vez y las actualizaciones completas se realizan con operaciones atómicas. Si dos hilos compiten, uno reintenta su transformación sobre la versión más reciente.
- **Configuración inmutable:** en vez de modificar propiedades individuales, se sustituye una instantánea completa. Esto evita estados intermedios incoherentes.
- **Responsabilidades separadas:** el estado, el contrato, la coordinación global y los consumidores viven en tipos distintos.
- **Validación centralizada:** cada instantánea nueva se valida antes de publicarse.
- **Pruebas deterministas:** las pruebas que comparten el acceso global se ejecutan secuencialmente, mientras que cada prueba crea datos únicos y las pruebas internas ejercitan concurrencia real.

## Uso

```csharp
using GlobalConfigurationSystem.Configuration;

var configuration = GlobalConfiguration.Current;

configuration.Update(current => current with
{
    EnvironmentName = "Staging",
    ServiceEndpoint = new Uri("https://staging.example.com/"),
    Revision = current.Revision + 1
});

Console.WriteLine(configuration.Snapshot.EnvironmentName);
```

## Compilar y probar

Desde este directorio:

```bash
dotnet restore GlobalConfigurationSystem.sln
dotnet build GlobalConfigurationSystem.sln --configuration Release --no-restore
dotnet test GlobalConfigurationSystem.sln --configuration Release --no-build
```

Las pruebas verifican el acceso desde módulos distintos, la identidad única, la ausencia de constructores públicos, el acceso concurrente y la conservación de cambios compartidos.
