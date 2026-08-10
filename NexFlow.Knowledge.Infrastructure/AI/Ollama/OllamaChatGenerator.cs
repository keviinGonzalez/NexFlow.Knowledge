using Microsoft.Extensions.Options;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Infrastructure.AI.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.AI.Ollama
{
    public sealed class OllamaChatGenerator : IChatGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaOptions _options;

        public OllamaChatGenerator(HttpClient httpClient, IOptions<OllamaOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var request = new OllamaGenerateRequest
            {
                Model = _options.ChatModel,
                Prompt = prompt,
                Stream = false
            };

            using var response = await _httpClient.PostAsJsonAsync("/api/generate", request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(cancellationToken);
            return result?.Response ?? throw new InvalidOperationException("Ollama no devolvió una respuesta.");
        }
    }
}
