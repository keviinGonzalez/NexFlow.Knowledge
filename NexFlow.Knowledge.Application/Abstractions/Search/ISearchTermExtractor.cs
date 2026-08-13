using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Search
{
    public interface ISearchTermExtractor
    {
        IReadOnlyList<string> Extract(string question);
    }
}
