using NexFlow.Knowledge.Domain.Base;
using NexFlow.Knowledge.Domain.Exceptions;
using NexFlow.Knowledge.Domain.Guards;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Text;


namespace NexFlow.Knowledge.Domain.Entities
{
    public sealed class DocumentChunk : BaseEntity
    {
        public Guid DocumentId { get; private set; }
        public string Content { get; private set; }
        public int ChunkIndex { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Vector? Embedding { get; private set; }


        public Document Document { get; private set; } = null!;

        private DocumentChunk()
        {
        }

        public DocumentChunk(Guid documentId, string content, int chunkIndex)
        {
            Guard.ValidateRequired(documentId, nameof(documentId));
            Guard.ValidateRequired(content, nameof(content));

            if (chunkIndex < 0)
                throw new DomainException("El índice del chunk no puede ser negativo.");

            DocumentId = documentId;
            Content = content.Trim();
            ChunkIndex = chunkIndex;
            CreatedAt = DateTime.UtcNow;
        }

        public static DocumentChunk Create(Guid documentId, string content, int chunkIndex)
        {
            return new DocumentChunk(documentId, content, chunkIndex);
        }

        public void SetEmbedding(Vector embedding)
        {
            Guard.ValidateRequired(embedding, nameof(embedding));

            Embedding = embedding;
        }
    }
}
