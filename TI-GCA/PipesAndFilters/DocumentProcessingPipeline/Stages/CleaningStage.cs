using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;

namespace DocumentProcessingPipeline.Stages;

public sealed class CleaningStage : IDocumentProcessingStage
{
    public string Name => "Limpieza";

    public Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();

        var emptyKeys = document.Metadata
            .Where(item => string.IsNullOrWhiteSpace(item.Value))
            .Select(item => item.Key)
            .ToArray();

        foreach (var key in emptyKeys)
        {
            document.Metadata.Remove(key);
        }

        foreach (var key in document.Metadata.Keys.ToArray())
        {
            document.Metadata[key] = document.Metadata[key].Trim();
        }

        document.IsClean = true;
        return Task.CompletedTask;
    }
}
