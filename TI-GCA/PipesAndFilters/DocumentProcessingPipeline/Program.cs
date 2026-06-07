using System.Text;
using DocumentProcessingPipeline.Domain;
using DocumentProcessingPipeline.Pipeline;
using DocumentProcessingPipeline.Storage;

var storage = new InMemoryDocumentStorage();
var pipeline = DefaultPipelineFactory.Create(storage);
var document = new PdfDocument(
    "ejemplo.pdf",
    Encoding.UTF8.GetBytes("%PDF-1.7 Documento de ejemplo para indexar."),
    new Dictionary<string, string> { ["Autor"] = "  Equipo documental  " });

await pipeline.ProcessAsync(document);

Console.WriteLine($"Documento válido: {document.IsValid}");
Console.WriteLine($"Términos indexados: {string.Join(", ", document.IndexTerms)}");
Console.WriteLine($"Ubicación: {document.StorageLocation}");
