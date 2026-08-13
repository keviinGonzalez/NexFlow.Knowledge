using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.AskKnowledge
{
    //public sealed record AskKnowledgeResponse(
    //    string Question,
    //    string Answer,
    //    IReadOnlyList<AskKnowledgeContextChunk> ContextChunks);
    public sealed record AskKnowledgeResponse(string Question, string Answer);

    public sealed record AskKnowledgeContextChunk(int ChunkIndex, double SemanticScore, double TextScore, double FinalScore,
        string Content);
}
