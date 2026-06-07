using DocumentProcessingPipeline.Domain;

namespace DocumentProcessingPipeline.Pipeline;

public interface IDocumentProcessingStage
{
    string Name { get; }

    Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default);
}
