using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Responses;
using NexFlow.Knowledge.Application.Abstractions.Search;
using NexFlow.Knowledge.Domain.Repositories;

namespace NexFlow.Knowledge.Application.Services.Search
{
    public sealed class HybridKnowledgeRetriever : IHybridKnowledgeRetriever
    {
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly ISearchTermExtractor _searchTermExtractor;
        private readonly ISearchResultScorer _searchResultScorer;

        public HybridKnowledgeRetriever(
            IEmbeddingGenerator embeddingGenerator,
            IDocumentChunkRepository documentChunkRepository,
            ISearchTermExtractor searchTermExtractor,
            ISearchResultScorer searchResultScorer)
        {
            _embeddingGenerator = embeddingGenerator;
            _documentChunkRepository = documentChunkRepository;
            _searchTermExtractor = searchTermExtractor;
            _searchResultScorer = searchResultScorer;
        }

        public async Task<IReadOnlyList<HybridSearchResult>> RetrieveAsync(
            string question,
            int sourceLimit,
            int resultLimit,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(question);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sourceLimit);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(resultLimit);

            var embedding = await _embeddingGenerator.GenerateAsync(
                question,
                cancellationToken);

            var semanticResults = await _documentChunkRepository.SearchSimilarAsync(
                embedding,
                sourceLimit,
                cancellationToken);

            var searchTerms = _searchTermExtractor.Extract(question);

            var textResults = await _documentChunkRepository.SearchTextAsync(
                searchTerms,
                sourceLimit,
                cancellationToken);

            var candidates = new Dictionary<(Guid DocumentId, int ChunkIndex), SearchCandidate>();

            foreach (var result in semanticResults)
            {
                var key = (result.Chunk.DocumentId, result.Chunk.ChunkIndex);

                candidates[key] = new SearchCandidate(
                    result.Chunk.DocumentId,
                    result.Chunk.ChunkIndex,
                    result.Chunk.Content,
                    result.Similarity,
                    0);
            }

            foreach (var result in textResults)
            {
                var key = (result.Chunk.DocumentId, result.Chunk.ChunkIndex);

                if (candidates.TryGetValue(key, out var existing))
                {
                    candidates[key] = existing with { TextScore = result.TextScore };
                    continue;
                }

                candidates[key] = new SearchCandidate(
                    result.Chunk.DocumentId,
                    result.Chunk.ChunkIndex,
                    result.Chunk.Content,
                    0,
                    result.TextScore);
            }

            return candidates.Values
                .Select(candidate => new HybridSearchResult(
                    candidate.DocumentId,
                    candidate.ChunkIndex,
                    candidate.Content,
                    candidate.SemanticScore,
                    candidate.TextScore,
                    _searchResultScorer.Calculate(
                        candidate.SemanticScore,
                        candidate.TextScore)))
                .OrderByDescending(result => result.FinalScore)
                .ThenByDescending(result => result.SemanticScore)
                .ThenByDescending(result => result.TextScore)
                .Take(resultLimit)
                .ToList();
        }
    }
}
