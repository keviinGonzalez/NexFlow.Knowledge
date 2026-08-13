using NexFlow.Knowledge.Domain.Entities;
using NexFlow.Knowledge.Domain.Repositories.Search;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Repositories
{
    public interface IDocumentChunkRepository
    {
        Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SimilarChunkResult>> SearchSimilarAsync(Vector embedding, int limit, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DocumentChunk>> SearchTextAsync(IReadOnlyList<string> searchTerms, int limit, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<DocumentChunk>> GetContextAsync(Guid documentId, int centerChunkIndex, int radius, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DocumentChunk>> SearchHybridAsync(string searchText, Vector embedding, int limit, CancellationToken cancellationToken = default);
    }
}
