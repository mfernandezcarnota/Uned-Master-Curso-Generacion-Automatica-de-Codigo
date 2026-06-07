using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;

namespace DocumentProcessingPipeline.Stages;

public sealed class ReceptionStage : IDocumentProcessingStage
{
    public string Name => "Recepción";

    public Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();
        document.IsReceived = true;
        return Task.CompletedTask;
    }
}
