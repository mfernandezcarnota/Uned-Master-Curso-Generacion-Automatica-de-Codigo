using System.Text;
using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;
using DocumentProcessingPipeline.Stages;

namespace DocumentProcessingPipeline.Tests;

public sealed class ValidationTests
{
    [Fact]
    public async Task ProcessAsync_WithPdfDocument_MarksItAsValid()
    {
        var document = TestDocumentFactory.CreateValid();
        var pipeline = new DocumentPipeline([new ValidationStage()]);

        await pipeline.ProcessAsync(document);

        Assert.True(document.IsValid);
    }

    [Theory]
    [InlineData("informe.txt", "%PDF-1.7 contenido")]
    [InlineData("informe.pdf", "contenido sin firma")]
    public async Task ProcessAsync_WithInvalidDocument_ThrowsException(string fileName, string content)
    {
        var document = new PdfDocument(fileName, Encoding.UTF8.GetBytes(content));
        var pipeline = new DocumentPipeline([new ValidationStage()]);

        await Assert.ThrowsAsync<InvalidDocumentException>(() => pipeline.ProcessAsync(document));

        Assert.False(document.IsValid);
    }
}
