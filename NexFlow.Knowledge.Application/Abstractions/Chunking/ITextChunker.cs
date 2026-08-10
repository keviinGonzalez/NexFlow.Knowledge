using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Chunking
{
    public interface ITextChunker
    {
        IReadOnlyList<string> Chunk(string text);
    }
}
