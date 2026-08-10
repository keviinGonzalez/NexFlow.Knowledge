using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexFlow.Knowledge.Domain.Entities;
using NexFlow.Knowledge.Domain.Repositories;
using NexFlow.Knowledge.Domain.Repositories.Search;
using NexFlow.Knowledge.Infrastructure.Persistence.Context;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Persistence.Repositories
{
    public class DocumentChunkRepository : IDocumentChunkRepository
    {
        private readonly AppDbContext _context;

        public DocumentChunkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(chunks, cancellationToken);
        }

        public async Task<IReadOnlyList<SimilarChunkResult>> SearchSimilarAsync(Vector embedding, int limit, CancellationToken cancellationToken = default)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "El límite debe ser mayor que cero.");

            var results = await _context.DocumentChunks
                .Where(x => x.Embedding != null)
               .Select(x => new
               {
                   Chunk = x,
                   Distance = x.Embedding!.CosineDistance(embedding)
               })
               .OrderBy(x => x.Distance)
               .Take(limit)
               .ToListAsync(cancellationToken);

            return results.Select(x => new SimilarChunkResult(x.Chunk, 1 - x.Distance)).ToList();
        }

        public async Task<IReadOnlyList<DocumentChunk>> SearchTextAsync(string searchText, int limit, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return [];

            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "El límite debe ser mayor que cero.");

            var normalizedSearchText = searchText.Trim();

            return await _context.DocumentChunks
                .Where(x => x.Content.Contains(normalizedSearchText))
                .OrderBy(x => x.ChunkIndex)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
    }
}
