using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;
using DocumentProcessingPipeline.Storage;

namespace DocumentProcessingPipeline.Tests;

public sealed class CompleteProcessingTests
{
    [Fact]
    public async Task DefaultPipeline_ProcessesDocumentThroughEveryStage()
    {
        var storage = new InMemoryDocumentStorage();
        var metadata = new Dictionary<string, string>
        {
            ["Autor"] = "  Ana  ",
            ["Descripción"] = "   "
        };
        var document = new PdfDocument(
            "informe.pdf",
            System.Text.Encoding.UTF8.GetBytes("%PDF-1.7 Informe anual ventas"),
            metadata);
        var pipeline = DefaultPipelineFactory.Create(storage);

        await pipeline.ProcessAsync(document);

        Assert.True(document.IsReceived);
        Assert.True(document.IsValid);
        Assert.True(document.IsClean);
        Assert.Equal("Ana", document.Metadata["Autor"]);
        Assert.False(document.Metadata.ContainsKey("Descripción"));
        Assert.Contains("informe", document.IndexTerms);
        Assert.Contains("ventas", document.IndexTerms);
        Assert.NotNull(document.StorageLocation);
        Assert.True(storage.TryGet(document.Id, out var storedDocument));
        Assert.Same(document, storedDocument);
    }
}
