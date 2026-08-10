using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Chunking
{
    public sealed class DocumentChunkService : IDocumentChunkService
    {
        private readonly ITextChunker _textChunker;
        private readonly IEmbeddingGenerator _embeddingGenerator;

        public DocumentChunkService(ITextChunker textChunker, IEmbeddingGenerator embeddingGenerator)
        {
            _textChunker = textChunker;
            _embeddingGenerator = embeddingGenerator;
        }

        public async Task<IReadOnlyList<DocumentChunk>> CreateChunksAsync(Document document, string extractedText, CancellationToken cancellationToken)
        {
            var textChunks = _textChunker.Chunk(extractedText);
            var documentChunks = new List<DocumentChunk>();

            for (var i = 0; i < textChunks.Count; i++)
            {
                var chunk = DocumentChunk.Create(document.Id, textChunks[i], i);
                var embedding = await _embeddingGenerator.GenerateAsync(textChunks[i], cancellationToken);

                chunk.SetEmbedding(embedding);
                documentChunks.Add(chunk);
            }

            return documentChunks;
        }
    }
}
