namespace DocumentProcessingPipeline.Stages;

public sealed class InvalidDocumentException : Exception
{
    public InvalidDocumentException(string message)
        : base(message)
    {
    }
}
