using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public sealed class ChunkOptions
    {
        public int ChunkSize { get; init; }
        public int Overlap { get; init; }
    }
}
