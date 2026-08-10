using Microsoft.EntityFrameworkCore;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Application.Abstractions.Responses;
using NexFlow.Knowledge.Domain.Entities;
using NexFlow.Knowledge.Domain.Repositories;
using NexFlow.Knowledge.Domain.Repositories.Search;
using NexFlow.Knowledge.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Persistence.Services
{
    public sealed class ChunkContextExpander : IChunkContextExpander
    {
        private readonly AppDbContext _context;

        public ChunkContextExpander(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ContextChunkResult>> ExpandAsync(
     IReadOnlyList<SimilarChunkResult> results,
     int radius,
     CancellationToken cancellationToken = default)
        {
            if (results.Count == 0)
                return [];

            if (radius < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(radius),
                    "El radio no puede ser negativo.");

            var documentIds = results
                .Select(x => x.Chunk.DocumentId)
                .Distinct()
                .ToList();

            var chunks = await _context.DocumentChunks
                .Where(x => documentIds.Contains(x.DocumentId))
                .OrderBy(x => x.DocumentId)
                .ThenBy(x => x.ChunkIndex)
                .ToListAsync(cancellationToken);

            var relevantChunks = results
                .ToDictionary(
                    x => (x.Chunk.DocumentId, x.Chunk.ChunkIndex),
                    x => x.Similarity);

            var expandedChunks = new Dictionary<
                (Guid DocumentId, int ChunkIndex),
                ContextChunkResult>();

            foreach (var result in results)
            {
                var documentId = result.Chunk.DocumentId;
                var chunkIndex = result.Chunk.ChunkIndex;

                var start = Math.Max(0, chunkIndex - radius);
                var end = chunkIndex + radius;

                foreach (var chunk in chunks.Where(x =>
                    x.DocumentId == documentId &&
                    x.ChunkIndex >= start &&
                    x.ChunkIndex <= end))
                {
                    var key = (chunk.DocumentId, chunk.ChunkIndex);

                    if (relevantChunks.TryGetValue(key, out var similarity))
                    {
                        expandedChunks[key] = new ContextChunkResult(
                            chunk,
                            similarity,
                            true);
                    }
                    else if (!expandedChunks.ContainsKey(key))
                    {
                        expandedChunks[key] = new ContextChunkResult(
                            chunk,
                            0,
                            false);
                    }
                }
            }

            return expandedChunks.Values
                .OrderBy(x => x.Chunk.DocumentId)
                .ThenBy(x => x.Chunk.ChunkIndex)
                .ToList();
        }
    }
}
