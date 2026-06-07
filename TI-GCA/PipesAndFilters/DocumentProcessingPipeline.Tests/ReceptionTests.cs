using DocumentProcessingPipeline.Pipeline;
using DocumentProcessingPipeline.Stages;

namespace DocumentProcessingPipeline.Tests;

public sealed class ReceptionTests
{
    [Fact]
    public async Task ProcessAsync_WithDocument_MarksItAsReceived()
    {
        var document = TestDocumentFactory.CreateValid();
        var pipeline = new DocumentPipeline([new ReceptionStage()]);

        var result = await pipeline.ProcessAsync(document);

        Assert.Same(document, result);
        Assert.True(result.IsReceived);
    }
}
