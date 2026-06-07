using DocumentProcessingPipeline.Stages;
using DocumentProcessingPipeline.Storage;

namespace DocumentProcessingPipeline.Pipeline;

public static class DefaultPipelineFactory
{
    public static DocumentPipeline Create(IDocumentStorage storage) => new(
    [
        new ReceptionStage(),
        new ValidationStage(),
        new CleaningStage(),
        new IndexingStage(),
        new StorageStage(storage)
    ]);
}
