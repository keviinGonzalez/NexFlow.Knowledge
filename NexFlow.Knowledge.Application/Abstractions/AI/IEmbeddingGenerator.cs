using Pgvector;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.AI
{
    public interface IEmbeddingGenerator
    {
        Task<Vector> GenerateAsync(string text, CancellationToken cancellationToken = default);
    }
}
