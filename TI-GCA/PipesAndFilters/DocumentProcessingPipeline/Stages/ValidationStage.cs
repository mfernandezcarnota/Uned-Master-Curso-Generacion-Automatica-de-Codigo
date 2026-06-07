using System.Text;
using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;

namespace DocumentProcessingPipeline.Stages;

public sealed class ValidationStage : IDocumentProcessingStage
{
    private const string PdfSignature = "%PDF-";

    public string Name => "Validación";

    public Task ProcessAsync(PdfDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        cancellationToken.ThrowIfCancellationRequested();

        if (!document.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDocumentException("El documento debe tener extensión .pdf.");
        }

        if (document.Content.Length < PdfSignature.Length ||
            Encoding.ASCII.GetString(document.Content.Span[..PdfSignature.Length]) != PdfSignature)
        {
            throw new InvalidDocumentException("El contenido no tiene una firma PDF válida.");
        }

        document.IsValid = true;
        return Task.CompletedTask;
    }
}
