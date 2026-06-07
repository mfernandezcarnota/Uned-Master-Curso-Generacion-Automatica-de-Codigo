using DocumentProcessingPipeline.Domain;

namespace DocumentProcessingPipeline.Storage;

public interface IDocumentStorage
{
    Task<string> SaveAsync(PdfDocument document, CancellationToken cancellationToken = default);
}
