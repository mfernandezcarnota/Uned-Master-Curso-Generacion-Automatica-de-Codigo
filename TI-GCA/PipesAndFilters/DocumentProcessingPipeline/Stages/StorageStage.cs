using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;
using DocumentProcessingPipeline.Storage;

namespace DocumentProcessingPipeline.Stages;

public sealed class StorageStage : IDocumentProcessingStage
{
    private readonly IDocumentStorage _storage;

    public StorageStage(IDocumentStorage storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public string Name => "Almacenamiento";

    public async Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        document.StorageLocation = await _storage.SaveAsync(document, cancellationToken);
    }
}
