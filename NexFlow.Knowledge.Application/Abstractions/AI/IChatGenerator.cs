using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.AI
{
    public interface IChatGenerator
    {
        Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
