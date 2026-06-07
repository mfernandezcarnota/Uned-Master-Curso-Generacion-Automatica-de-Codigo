using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;
using DocumentProcessingPipeline.Stages;

namespace DocumentProcessingPipeline.Tests;

public sealed class PipelineConfigurationTests
{
    [Fact]
    public async Task AddStage_IncorporatesStageIntoProcessing()
    {
        var customStage = new AuditStage();
        var pipeline = new DocumentPipeline([new ReceptionStage()]);
        pipeline.AddStage(customStage);
        var document = TestDocumentFactory.CreateValid();

        await pipeline.ProcessAsync(document);

        Assert.Equal(1, customStage.ProcessedDocuments);
        Assert.Equal(new[] { "Recepción", "Auditoría" }, pipeline.Stages.Select(stage => stage.Name));
    }

    [Fact]
    public async Task RemoveStage_ExcludesStageFromProcessing()
    {
        var validation = new ValidationStage();
        var customStage = new AuditStage();
        var pipeline = new DocumentPipeline([validation, customStage]);

        var removed = pipeline.RemoveStage(customStage.Name);
        await pipeline.ProcessAsync(TestDocumentFactory.CreateValid());

        Assert.True(removed);
        Assert.Equal(0, customStage.ProcessedDocuments);
        Assert.Single(pipeline.Stages);
        Assert.Same(validation, pipeline.Stages[0]);
    }

    [Fact]
    public void Configure_ReplacesAndReordersAllStages()
    {
        var pipeline = new DocumentPipeline([new ReceptionStage(), new ValidationStage()]);

        pipeline.Configure([new CleaningStage(), new ReceptionStage()]);

        Assert.Equal(new[] { "Limpieza", "Recepción" }, pipeline.Stages.Select(stage => stage.Name));
    }

    private sealed class AuditStage : IDocumentProcessingStage
    {
        public string Name => "Auditoría";

        public int ProcessedDocuments { get; private set; }

        public Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
        {
            ProcessedDocuments++;
            return Task.CompletedTask;
        }
    }
}
