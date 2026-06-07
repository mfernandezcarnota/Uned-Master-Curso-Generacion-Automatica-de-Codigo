using System.Text;
using DocumentProcessingPipeline.Domain;

namespace DocumentProcessingPipeline.Tests;

internal static class TestDocumentFactory
{
    public static PdfDocument CreateValid(
        string fileName = "informe.pdf",
        string content = "%PDF-1.7 Informe anual de ejemplo") =>
        new(fileName, Encoding.UTF8.GetBytes(content));
}
