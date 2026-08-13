using NexFlow.Knowledge.Domain.Repositories.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public interface IChunkReranker
    {
        IReadOnlyList<SimilarChunkResult> Rerank(string question, IReadOnlyList<SimilarChunkResult> candidates);
    }
}
