using System.Text;
using System.Text.RegularExpressions;
using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;

namespace DocumentProcessingPipeline.Stages;

public sealed partial class IndexingStage : IDocumentProcessingStage
{
    public string Name => "Indexación";

    public Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();

        var text = Encoding.UTF8.GetString(document.Content.Span);
        document.IndexTerms = WordRegex()
            .Matches(text)
            .Cast<Match>()
            .Select(match => match.Value.ToLowerInvariant())
            .Where(term => term.Length > 2)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        return Task.CompletedTask;
    }

    [GeneratedRegex(@"[\p{L}\p{N}]+")]
    private static partial Regex WordRegex();
}
