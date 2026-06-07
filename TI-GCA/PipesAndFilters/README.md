# DocumentProcessingPipeline

Solución en C# y .NET 8 para procesar documentos PDF mediante una secuencia configurable de etapas independientes.

## Funcionalidad

La configuración inicial ejecuta, en orden:

1. **Recepción**: registra la entrada del documento.
2. **Validación**: comprueba la extensión y la firma del contenido PDF.
3. **Limpieza**: normaliza los metadatos y elimina valores vacíos.
4. **Indexación**: genera términos normalizados a partir del contenido.
5. **Almacenamiento**: guarda el documento mediante una abstracción de persistencia.

`DocumentPipeline` permite añadir e insertar etapas, eliminarlas por nombre y reemplazar por completo la configuración. Cada ejecución usa una instantánea de las etapas configuradas, de modo que una reconfiguración concurrente solo afecta a ejecuciones posteriores.

## Estructura

```text
./
├── DocumentProcessingPipeline.sln
├── DocumentProcessingPipeline/
│   ├── Domain/              # Documento PDF y estado de procesamiento
│   ├── Pipeline/            # Contrato, coordinación y configuración predeterminada
│   ├── Stages/              # Etapas iniciales
│   ├── Storage/             # Contrato y almacenamiento en memoria
│   └── Program.cs           # Ejemplo ejecutable
├── DocumentProcessingPipeline.Tests/
│   ├── ReceptionTests.cs
│   ├── ValidationTests.cs
│   ├── CompleteProcessingTests.cs
│   └── PipelineConfigurationTests.cs
└── README.md
```

## Extensión y reconfiguración

Una etapa nueva solo necesita implementar `IDocumentProcessingStage`:

```csharp
public sealed class AuditStage : IDocumentProcessingStage
{
    public string Name => "Auditoría";

    public Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Procesado: {document.FileName}");
        return Task.CompletedTask;
    }
}
```

Después puede incorporarse o retirarse durante la composición:

```csharp
pipeline.AddStage(new AuditStage());
pipeline.RemoveStage("Auditoría");
pipeline.InsertStage(1, new AuditStage());
pipeline.RemoveStage("Auditoría");
pipeline.Configure([new ReceptionStage(), new ValidationStage()]);
```

Los nombres de las etapas deben ser únicos dentro de una configuración para que las operaciones sean deterministas.

## Ejecución

Desde este directorio:

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project DocumentProcessingPipeline
dotnet test --no-restore
```
