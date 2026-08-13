using NexFlow.Knowledge.Domain.Repositories.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public interface IContextGroupBuilder
    {
        IReadOnlyList<ContextChunkGroup> Build(IReadOnlyList<SimilarChunkResult> results, int radius);
    }
}
