namespace NexFlow.Knowledge.Application.Abstractions.Search
{
    public interface IHybridKnowledgeRetriever
    {
        Task<IReadOnlyList<HybridSearchResult>> RetrieveAsync(
            string question,
            int sourceLimit,
            int resultLimit,
            CancellationToken cancellationToken = default);
    }
}
