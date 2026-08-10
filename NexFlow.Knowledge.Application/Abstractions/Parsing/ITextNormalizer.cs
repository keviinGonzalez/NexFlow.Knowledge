using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Parsing
{
    public interface ITextNormalizer
    {
        string Normalize(string text);
    }
}
