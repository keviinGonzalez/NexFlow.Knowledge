using MediatR;
using Microsoft.Extensions.Options;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Options;
using NexFlow.Knowledge.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.AskKnowledge
{
    public sealed class AskKnowledgeHandler : IRequestHandler<AskKnowledgeQuery, AskKnowledgeResponse>
    {
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly IChatGenerator _chatGenerator;
        private readonly KnowledgeOptions _options;

        public AskKnowledgeHandler(IEmbeddingGenerator embeddingGenerator, IDocumentChunkRepository documentChunkRepository,
            IChatGenerator chatGenerator, IOptions<KnowledgeOptions> options)
        {
            _embeddingGenerator = embeddingGenerator;
            _documentChunkRepository = documentChunkRepository;
            _chatGenerator = chatGenerator;
            _options = options.Value;
        }

        public async Task<AskKnowledgeResponse> Handle(AskKnowledgeQuery request, CancellationToken cancellationToken)
        {
            var embedding = await _embeddingGenerator.GenerateAsync(request.Question, cancellationToken);
            var chunks = await _documentChunkRepository.SearchSimilarAsync(embedding, _options.RetrievalLimit, cancellationToken);
            var relevantChunks = chunks
                .Where(x => x.Similarity >= _options.SimilarityThreshold)
                .OrderByDescending(x => x.Similarity)
                .Take(_options.ContextLimit)
                .ToList();

            if (relevantChunks.Count == 0)
            {
                return new AskKnowledgeResponse(
                    request.Question,
                    "No se encontró información suficiente en los documentos para responder la pregunta.");
            }

            var context = string.Join(Environment.NewLine + Environment.NewLine,
                relevantChunks.Select((x, index) =>
                $"""
                [Fragmento {index + 1}]
                {x.Chunk.Content}
                """));

            var prompt = $"""
            Eres un asistente especializado en normativa de tránsito.

            Responde la pregunta utilizando únicamente la información
            proporcionada en el contexto.

            Si el contexto no contiene información suficiente para responder,
            indícalo claramente y no inventes información.

            Contexto:
            {context}

            Pregunta:
            {request.Question}

            Respuesta:
            """;

            var answer = await _chatGenerator.GenerateAsync(prompt, cancellationToken);
            return new AskKnowledgeResponse(request.Question, answer);
        }
    }
}
