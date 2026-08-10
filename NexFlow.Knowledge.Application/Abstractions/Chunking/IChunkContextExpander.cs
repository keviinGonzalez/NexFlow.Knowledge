using NexFlow.Knowledge.Application.Abstractions.Responses;
using NexFlow.Knowledge.Domain.Repositories.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public interface IChunkContextExpander
    {
        Task<IReadOnlyList<ContextChunkResult>> ExpandAsync(IReadOnlyList<SimilarChunkResult> results, int radius, CancellationToken cancellationToken = default);
    }
}
