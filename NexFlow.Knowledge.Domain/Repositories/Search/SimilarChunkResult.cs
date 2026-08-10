using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Repositories.Search
{
    public sealed record SimilarChunkResult(DocumentChunk Chunk, double Similarity);
}
