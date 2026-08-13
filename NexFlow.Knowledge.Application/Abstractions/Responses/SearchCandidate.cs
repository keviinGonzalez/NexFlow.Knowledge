using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Responses
{
    public sealed record SearchCandidate(Guid DocumentId, int ChunkIndex, string Content, double SemanticScore, double TextScore);
}
