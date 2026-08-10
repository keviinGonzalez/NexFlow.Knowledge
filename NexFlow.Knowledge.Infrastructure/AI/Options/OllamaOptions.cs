using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.AI.Options
{
    public sealed class OllamaOptions
    {
        public const string SectionName = "Ollama";
        public string BaseUrl { get; init; } = string.Empty;
        public string ChatModel { get; init; } = string.Empty;
        public string EmbeddingModel { get; init; } = string.Empty;
    }
}
