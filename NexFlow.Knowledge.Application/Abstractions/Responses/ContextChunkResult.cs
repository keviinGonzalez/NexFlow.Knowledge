using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Responses
{
    public sealed record ContextChunkResult(DocumentChunk Chunk, double Similarity, bool IsOriginal);
}
