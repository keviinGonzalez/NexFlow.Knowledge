using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Parsing
{
    public interface IDocumentParser
    {
        Task<string> ExtractTextAsync(Stream content, CancellationToken cancellationToken = default);
    }
}
