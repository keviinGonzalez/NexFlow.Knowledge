using MediatR;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Domain.Repositories;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.SearchKnowledge
{
    public sealed class SearchKnowledgeHandler : IRequestHandler<SearchKnowledgeQuery, SearchKnowledgeResponse>
    {
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly IChunkContextExpander _chunkContextExpander;

        public SearchKnowledgeHandler(IEmbeddingGenerator embeddingGenerator, IDocumentChunkRepository documentChunkRepository, IChunkContextExpander chunkContextExpander)
        {
            _embeddingGenerator = embeddingGenerator;
            _documentChunkRepository = documentChunkRepository;
            _chunkContextExpander = chunkContextExpander;
        }
        public async Task<SearchKnowledgeResponse> Handle(SearchKnowledgeQuery request, CancellationToken cancellationToken)
        {
            var embedding = await _embeddingGenerator.GenerateAsync(request.Question, cancellationToken);

            var chunks = await _documentChunkRepository.SearchSimilarAsync(embedding, 5, cancellationToken);

            // 2. Nos quedamos únicamente con resultados suficientemente relevantes.
            var relevantChunks = chunks
                .Where(x => x.Similarity >= 0.70)
                .OrderByDescending(x => x.Similarity)
                .Take(3)
                .ToList();

            if (relevantChunks.Count == 0)
            {
                return new SearchKnowledgeResponse(
                    request.Question,
                    []);
            }

            //Expandimos el contexto de los resultados relevantes.
            var contextChunks = await _chunkContextExpander.ExpandAsync(relevantChunks, 1, cancellationToken);
            // Convertimos el contexto recuperado en resultados.
            var results = contextChunks
                .Select(x => new SearchKnowledgeResult(
                    x.Chunk.DocumentId,
                    x.Chunk.ChunkIndex,
                    x.Chunk.Content,
                    x.Similarity))
                .ToList();

            return new SearchKnowledgeResponse(
                request.Question,
                results);
        }
    }
}
