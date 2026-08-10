using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public interface IDocumentChunkService
    {
        Task<IReadOnlyList<DocumentChunk>> CreateChunksAsync(Document document, string extractedText, CancellationToken cancellationToken = default);
    }
}
