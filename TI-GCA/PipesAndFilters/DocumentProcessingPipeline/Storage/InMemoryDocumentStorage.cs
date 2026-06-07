using System.Collections.Concurrent;
using DocumentProcessingPipeline.Domain;

namespace DocumentProcessingPipeline.Storage;

public sealed class InMemoryDocumentStorage : IDocumentStorage
{
    private readonly ConcurrentDictionary<Guid, PdfDocument> _documents = new();

    public int Count => _documents.Count;

    public Task<string> SaveAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();
        _documents[document.Id] = document;
        return Task.FromResult($"memory://documents/{document.Id}");
    }

    public bool TryGet(Guid id, out PdfDocument? document) => _documents.TryGetValue(id, out document);
}
