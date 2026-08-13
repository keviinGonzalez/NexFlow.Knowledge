using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.SearchKnowledge
{
    public sealed record SearchKnowledgeResponse(string Question, IReadOnlyList<SearchKnowledgeResult> Results);
    public sealed record SearchKnowledgeResult(
        Guid DocumentId,
        int ChunkIndex,
        string Content,
        double SemanticScore,
        double TextScore,
        double FinalScore);
}
