using DocumentProcessingPipeline.Domain;

namespace DocumentProcessingPipeline.Pipeline;

public sealed class DocumentPipeline
{
    private readonly object _sync = new();
    private List<IDocumentProcessingStage> _stages;

    public DocumentPipeline(IEnumerable<IDocumentProcessingStage>? stages = null)
    {
        _stages = ValidateStages(stages ?? Array.Empty<IDocumentProcessingStage>());
    }

    public IReadOnlyList<IDocumentProcessingStage> Stages
    {
        get
        {
            lock (_sync)
            {
                return _stages.ToArray();
            }
        }
    }

    public DocumentPipeline AddStage(IDocumentProcessingStage stage)
    {
        ArgumentNullException.ThrowIfNull(stage);

        lock (_sync)
        {
            EnsureUniqueName(stage.Name, _stages);
            _stages.Add(stage);
        }

        return this;
    }

    public DocumentPipeline InsertStage(int index, IDocumentProcessingStage stage)
    {
        ArgumentNullException.ThrowIfNull(stage);

        lock (_sync)
        {
            EnsureUniqueName(stage.Name, _stages);
            _stages.Insert(index, stage);
        }

        return this;
    }

    public bool RemoveStage(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        lock (_sync)
        {
            var index = _stages.FindIndex(stage => string.Equals(stage.Name, name, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return false;
            }

            _stages.RemoveAt(index);
            return true;
        }
    }

    public DocumentPipeline Configure(IEnumerable<IDocumentProcessingStage> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        var replacement = ValidateStages(stages);

        lock (_sync)
        {
            _stages = replacement;
        }

        return this;
    }

    public async Task<PdfDocument> ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        IDocumentProcessingStage[] stages;
        lock (_sync)
        {
            stages = _stages.ToArray();
        }

        foreach (var stage in stages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await stage.ProcessAsync(document, cancellationToken);
        }

        return document;
    }

    private static List<IDocumentProcessingStage> ValidateStages(IEnumerable<IDocumentProcessingStage> stages)
    {
        var result = new List<IDocumentProcessingStage>();
        foreach (var stage in stages)
        {
            ArgumentNullException.ThrowIfNull(stage);
            EnsureUniqueName(stage.Name, result);
            result.Add(stage);
        }

        return result;
    }

    private static void EnsureUniqueName(string name, IEnumerable<IDocumentProcessingStage> stages)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (stages.Any(stage => string.Equals(stage.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Ya existe una etapa con el nombre '{name}'.");
        }
    }
}
