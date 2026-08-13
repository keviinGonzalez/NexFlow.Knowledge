using NexFlow.Knowledge.Application.Abstractions.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public sealed record ContextChunkGroup(Guid DocumentId, IReadOnlyList<ContextChunkResult> Chunks, double Score);
}
