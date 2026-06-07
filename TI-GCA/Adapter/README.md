# BankIntegrationSystem

Solución de ejemplo en C# y .NET 8 que permite a una aplicación financiera consumir proveedores bancarios incompatibles mediante un contrato uniforme.

## Objetivos

- Mantener los casos de uso financieros independientes de cada proveedor.
- Traducir operaciones y convertir formatos específicos en los límites de la aplicación.
- Incorporar otra API bancaria sin modificar el servicio cliente.
- Sustituir una integración heredada por una moderna mediante composición.

## Arquitectura

La solución separa las responsabilidades en las siguientes áreas:

- **Domain**: modelos financieros comunes (`BankAccount`, `Money`, `TransferRequest` y `TransferReceipt`).
- **Application**: contrato `IBankGateway` y servicio cliente `BankingService`; esta capa no conoce APIs concretas.
- **Infrastructure/LegacyNorthBank**: integración con una API que usa cuentas sin guiones e importes en céntimos.
- **Infrastructure/LegacySouthBank**: integración con una segunda API que intercambia saldos y cantidades como texto.
- **Infrastructure/ModernBank**: integración sustituible con un proveedor que ofrece operaciones asíncronas modernas.

Las dependencias apuntan hacia el contrato de aplicación. Cada integración concentra la traducción de llamadas y formatos de su proveedor, de modo que añadir o sustituir proveedores no requiere cambiar `BankingService`.

## Estructura

```text
Adapter/
├── BankIntegrationSystem.sln
├── BankIntegrationSystem/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   │   ├── LegacyNorthBank/
│   │   ├── LegacySouthBank/
│   │   └── ModernBank/
│   └── Program.cs
├── BankIntegrationSystem.Tests/
│   └── BankIntegrationTests.cs
└── README.md
```

## Extensión con otro proveedor

1. Crear una clase de infraestructura que implemente `IBankGateway`.
2. Inyectar la API específica del proveedor en esa clase.
3. Encapsular allí la traducción de llamadas, códigos de estado y formatos.
4. Entregar la nueva implementación a `BankingService` durante la composición de la aplicación.

No es necesario modificar el servicio cliente ni los modelos del dominio.

## Pruebas

`BankIntegrationSystem.Tests` verifica:

- La traducción de una transferencia uniforme a la operación requerida por North Bank.
- La conversión de saldos e importes textuales usados por South Bank.
- La incorporación de una segunda API heredada sin cambios en el cliente.
- La sustitución del proveedor heredado por una implementación moderna.

## Ejecución

Desde este directorio:

```bash
dotnet restore BankIntegrationSystem.sln
dotnet build BankIntegrationSystem.sln --no-restore
dotnet test BankIntegrationSystem.sln --no-build
dotnet run --project BankIntegrationSystem
```
