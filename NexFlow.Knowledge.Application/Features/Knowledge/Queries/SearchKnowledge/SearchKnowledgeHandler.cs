using MediatR;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Application.Abstractions.Responses;
using NexFlow.Knowledge.Application.Abstractions.Search;
using NexFlow.Knowledge.Domain.Entities;
using NexFlow.Knowledge.Domain.Repositories;
using NexFlow.Knowledge.Domain.Repositories.Search;
using Pgvector;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.SearchKnowledge
{
    public sealed class SearchKnowledgeHandler : IRequestHandler<SearchKnowledgeQuery, SearchKnowledgeResponse>
    {
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly IChunkContextExpander _chunkContextExpander;
        private readonly IChunkReranker _chunkReranker;
        private readonly ISearchTermExtractor _searchTermExtractor;
        private readonly ISearchResultScorer _searchResultScorer;

        public SearchKnowledgeHandler(IEmbeddingGenerator embeddingGenerator, IDocumentChunkRepository documentChunkRepository,
            IChunkContextExpander chunkContextExpander, IChunkReranker chunkReranker, ISearchTermExtractor searchTermExtractor
            , ISearchResultScorer searchResultScorer)
        {
            _embeddingGenerator = embeddingGenerator;
            _documentChunkRepository = documentChunkRepository;
            _chunkContextExpander = chunkContextExpander;
            _chunkReranker = chunkReranker;
            _searchTermExtractor = searchTermExtractor;
            _searchResultScorer = searchResultScorer;
        }


        public SearchKnowledgeHandler(
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

        public async Task<SearchKnowledgeResponse> Handle(
            SearchKnowledgeQuery request,
            CancellationToken cancellationToken)
        {
            // ============================================================
            // 1. EMBEDDING
            // ============================================================

            var embedding =
                await _embeddingGenerator.GenerateAsync(
                    request.Question,
                    cancellationToken);

            // ============================================================
            // 2. BÚSQUEDA SEMÁNTICA
            // ============================================================

            var semanticResults =
                await _documentChunkRepository.SearchSimilarAsync(
                    embedding,
                    10,
                    cancellationToken);

            // ============================================================
            // 3. EXTRACCIÓN DE TÉRMINOS
            // ============================================================

            var searchTerms =
                _searchTermExtractor.Extract(request.Question);

            Console.WriteLine("===== SEARCH TERMS =====");

            foreach (var term in searchTerms)
            {
                Console.WriteLine(term);
            }

            // ============================================================
            // 4. BÚSQUEDA TEXTUAL
            // ============================================================

            var textResults =
                await _documentChunkRepository.SearchTextAsync(
                    searchTerms,
                    10,
                    cancellationToken);

            return new SearchKnowledgeResponse(
    request.Question,
    textResults.Select(x =>
        new SearchKnowledgeResult(
            x.DocumentId,
            x.ChunkIndex,
            x.Content,
            1.0))
    .ToList());

            Console.WriteLine("===== TEXT RESULTS =====");

            foreach (var result in textResults)
            {
                Console.WriteLine(
                    $"Chunk: {result.ChunkIndex} | " +
                    $"Document: {result.DocumentId}");

                Console.WriteLine(result.Content);
                Console.WriteLine("--------------------------------");
            }

            // ============================================================
            // 5. COMBINAR RESULTADOS
            // ============================================================

            var candidates =
                new Dictionary<
                    (Guid DocumentId, int ChunkIndex),
                    SearchCandidate>();

            // ------------------------------------------------------------
            // 5.1 Resultados semánticos
            // ------------------------------------------------------------

            foreach (var result in semanticResults)
            {
                var key = (
                    result.Chunk.DocumentId,
                    result.Chunk.ChunkIndex);

                candidates[key] = new SearchCandidate(
                    result.Chunk.DocumentId,
                    result.Chunk.ChunkIndex,
                    result.Chunk.Content,
                    result.Similarity,
                    0);
            }

            // ------------------------------------------------------------
            // 5.2 Resultados textuales
            // ------------------------------------------------------------

            foreach (var result in textResults)
            {
                var key = (
                    result.DocumentId,
                    result.ChunkIndex);

                if (candidates.TryGetValue(key, out var existing))
                {
                    candidates[key] = existing with
                    {
                        TextScore = 1
                    };
                }
                else
                {
                    candidates[key] = new SearchCandidate(
                        result.DocumentId,
                        result.ChunkIndex,
                        result.Content,
                        0,
                        1);
                }
            }

            // ============================================================
            // 6. CALCULAR SCORE FINAL
            // ============================================================

            var results = candidates.Values
                .Select(x =>
                    new SearchKnowledgeResult(
                        x.DocumentId,
                        x.ChunkIndex,
                        x.Content,
                        _searchResultScorer.Calculate(
                            x.SemanticScore,
                            x.TextScore)))
                .OrderByDescending(x => x.Similarity)
                .Take(20)
                .ToList();

            // ============================================================
            // 7. RESPUESTA
            // ============================================================

            return new SearchKnowledgeResponse(
                request.Question,
                results);
        }


    }
}
