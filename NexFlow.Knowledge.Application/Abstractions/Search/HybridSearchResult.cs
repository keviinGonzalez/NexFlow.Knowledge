namespace NexFlow.Knowledge.Application.Abstractions.Search
{
    public sealed record HybridSearchResult(
        Guid DocumentId,
        int ChunkIndex,
        string Content,
        double SemanticScore,
        double TextScore,
        double FinalScore);
}
