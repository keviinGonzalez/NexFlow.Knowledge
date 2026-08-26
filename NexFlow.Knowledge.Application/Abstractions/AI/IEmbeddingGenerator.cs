using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.AI
{
    public interface IEmbeddingGenerator
    {
        Task<ReadOnlyMemory<float>> GenerateAsync(string text, CancellationToken cancellationToken = default);
    }
}
