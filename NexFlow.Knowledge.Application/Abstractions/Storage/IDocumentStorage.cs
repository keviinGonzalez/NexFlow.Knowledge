using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Storage
{
    public interface IDocumentStorage
    {
        Task<StoredDocument> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default);
    }
}
