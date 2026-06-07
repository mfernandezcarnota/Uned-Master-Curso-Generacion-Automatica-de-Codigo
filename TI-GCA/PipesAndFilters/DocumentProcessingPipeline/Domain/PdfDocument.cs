namespace DocumentProcessingPipeline.Domain;

public sealed class PdfDocument
{
    public PdfDocument(string fileName, ReadOnlyMemory<byte> content, IDictionary<string, string>? metadata = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        Id = Guid.NewGuid();
        FileName = fileName;
        Content = content;
        Metadata = metadata is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase);
    }

    public Guid Id { get; }

    public string FileName { get; }

    public ReadOnlyMemory<byte> Content { get; }

    public IDictionary<string, string> Metadata { get; }

    public bool IsReceived { get; internal set; }

    public bool IsValid { get; internal set; }

    public bool IsClean { get; internal set; }

    public IReadOnlyCollection<string> IndexTerms { get; internal set; } = Array.Empty<string>();

    public string? StorageLocation { get; internal set; }
}
