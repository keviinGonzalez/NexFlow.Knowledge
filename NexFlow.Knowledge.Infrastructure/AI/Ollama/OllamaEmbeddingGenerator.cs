using Microsoft.Extensions.Options;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Infrastructure.AI.Options;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.AI.Ollama
{
    public sealed class OllamaEmbeddingGenerator : IEmbeddingGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaOptions _options;

        public OllamaEmbeddingGenerator(HttpClient httpClient, IOptions<OllamaOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<ReadOnlyMemory<float>> GenerateAsync(string text, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));

            var request = new
            {
                model = _options.EmbeddingModel,
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync("/api/embed", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken);

            if (result?.Embeddings is null || result.Embeddings.Count == 0)
                throw new InvalidOperationException("Ollama no devolvió ningún embedding.");

            return new ReadOnlyMemory<float>(result.Embeddings[0]);
        }

        private sealed class OllamaEmbeddingResponse
        {
            public List<float[]> Embeddings { get; init; } = [];
        }
    }
}
