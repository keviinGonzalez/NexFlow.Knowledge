using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Repositories
{
    public interface IDocumentRepository
    {
        Task AddAsync(Document document, CancellationToken cancellationToken = default);
    }

}
