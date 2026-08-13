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

        public async Task<IReadOnlyList<TextSearchResult>> SearchTextAsync(IReadOnlyList<string> searchTerms, int limit, CancellationToken cancellationToken = default)
        {
            if (searchTerms is null || searchTerms.Count == 0)
                return [];

            if (limit <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    "El límite debe ser mayor que cero.");

            var terms = searchTerms
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (terms.Count == 0)
                return [];

            var patterns = terms
                .Select(term => $"%{term}%")
                .ToList();

            var matchingChunks = await _context.DocumentChunks
                .Where(chunk =>
                    patterns.Any(pattern => EF.Functions.ILike(chunk.Content, pattern)))
                .ToListAsync(cancellationToken);

            return matchingChunks
                .Select(chunk => new TextSearchResult(
                    chunk,
                    CalculateTextScore(chunk.Content, terms)))
                .Where(x => x.TextScore > 0)
                .OrderByDescending(x => x.TextScore)
                .ThenBy(x => x.Chunk.DocumentId)
                .ThenBy(x => x.Chunk.ChunkIndex)
                .Take(limit)
                .ToList();
        }

        private static double CalculateTextScore(
            string content,
            IReadOnlyList<string> terms)
        {
            if (string.IsNullOrWhiteSpace(content) || terms.Count == 0)
                return 0;

            var normalizedContent = content.ToUpperInvariant();
            var totalWeight = terms.Sum(GetTermWeight);

            if (totalWeight == 0)
                return 0;

            var matchedWeight = terms
                .Where(term => normalizedContent.Contains(
                    term.ToUpperInvariant(),
                    StringComparison.Ordinal))
                .Sum(GetTermWeight);

            return (double)matchedWeight / totalWeight;
        }

        private static int GetTermWeight(string term)
        {
            return term.Contains(' ') ? 2 : 1;
        }

        public async Task<IReadOnlyList<DocumentChunk>> GetContextAsync(Guid documentId, int chunkIndex, int radius, CancellationToken cancellationToken = default)
        {
            if (radius < 0)
            { throw new ArgumentOutOfRangeException(nameof(radius), "El radio no puede ser negativo."); }
            if (chunkIndex < 0) { throw new ArgumentOutOfRangeException(nameof(chunkIndex), "El índice del chunk no puede ser negativo."); }
            var startIndex = Math.Max(0, chunkIndex - radius); var endIndex = chunkIndex + radius;
            return await _context.DocumentChunks
                .Where(x => x.DocumentId == documentId &&
                    x.ChunkIndex >= startIndex && x.ChunkIndex <= endIndex)
                .OrderBy(x => x.ChunkIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<DocumentChunk>> SearchHybridAsync(
    string searchText,
    Vector embedding,
    int limit,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return [];

            if (limit <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    "El límite debe ser mayor que cero.");

            var semanticResults = await _context.DocumentChunks
                .Where(x => x.Embedding != null)
                .Select(x => new
                {
                    Chunk = x,
                    Distance = x.Embedding!.CosineDistance(embedding)
                })
                .OrderBy(x => x.Distance)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var normalizedSearchText = searchText.Trim();

            var textResults = await _context.DocumentChunks
                .Where(x => x.Content.Contains(normalizedSearchText))
                .Take(limit)
                .ToListAsync(cancellationToken);


            var results = semanticResults
                .Select(x => x.Chunk)
                .Concat(textResults)
                .GroupBy(x => new
                {
                    x.DocumentId,
                    x.ChunkIndex
                })
                .Select(x => x.First())
                .Take(limit)
                .ToList();

            return results;
        }
    }
}
