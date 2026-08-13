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
            {
                throw new ArgumentOutOfRangeException(
                    nameof(radius),
                    "El radio no puede ser negativo.");
            }

            // Documentos involucrados en los resultados semánticos.
            var documentIds = results
                .Select(x => x.Chunk.DocumentId)
                .Distinct()
                .ToList();

            // Recuperamos los chunks de esos documentos.
            //
            // En esta etapa todavía estamos trabajando solamente
            // con los documentos que tuvieron resultados semánticos.
            var chunks = await _context.DocumentChunks
                .Where(x => documentIds.Contains(x.DocumentId))
                .OrderBy(x => x.DocumentId)
                .ThenBy(x => x.ChunkIndex)
                .ToListAsync(cancellationToken);

            // Guardamos la similitud de los chunks que realmente
            // hicieron match con la pregunta.
            var similarityByChunk = results
                .GroupBy(x => (x.Chunk.DocumentId, x.Chunk.ChunkIndex))
                .ToDictionary(
                    x => x.Key,
                    x => x.Max(y => y.Similarity));

            var expandedChunks = new List<ContextChunkResult>();

            foreach (var result in results)
            {
                var documentId = result.Chunk.DocumentId;
                var chunkIndex = result.Chunk.ChunkIndex;

                var start = Math.Max(
                    0,
                    chunkIndex - radius);

                var end = chunkIndex + radius;

                var relatedChunks = chunks
                    .Where(x =>
                        x.DocumentId == documentId &&
                        x.ChunkIndex >= start &&
                        x.ChunkIndex <= end)
                    .OrderBy(x => x.ChunkIndex)
                    .ToList();

                foreach (var chunk in relatedChunks)
                {
                    var key = (
                        chunk.DocumentId,
                        chunk.ChunkIndex);

                    var isDirectMatch =
                        similarityByChunk.TryGetValue(
                            key,
                            out var similarity);

                    expandedChunks.Add(
                        new ContextChunkResult(
                            chunk,
                            isDirectMatch ? similarity : 0,
                            isDirectMatch));
                }
            }

            // Eliminamos duplicados.
            //
            // Puede ocurrir que:
            //
            // resultado 285 -> expanda 284,285,286
            // resultado 286 -> expanda 285,286,287
            //
            // En ese caso no queremos devolver dos veces
            // los mismos chunks.
            var distinctChunks = expandedChunks
                .GroupBy(x => (
                    x.Chunk.DocumentId,
                    x.Chunk.ChunkIndex))
                .Select(group =>
                    group
                        .OrderByDescending(x => x.IsDirectMatch)
                        .ThenByDescending(x => x.Similarity)
                        .First())
                .ToList();

            // Los resultados directos primero y posteriormente
            // sus vecinos/contexto.
            return distinctChunks
                .OrderByDescending(x => x.IsDirectMatch)
                .ThenByDescending(x => x.Similarity)
                .ThenBy(x => x.Chunk.DocumentId)
                .ThenBy(x => x.Chunk.ChunkIndex)
                .ToList();
        }

    }
}
