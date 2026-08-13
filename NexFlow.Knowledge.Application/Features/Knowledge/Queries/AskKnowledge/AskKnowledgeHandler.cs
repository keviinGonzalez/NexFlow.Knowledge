using MediatR;
using Microsoft.Extensions.Options;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Options;
using NexFlow.Knowledge.Application.Abstractions.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.AskKnowledge
{
    public sealed class AskKnowledgeHandler : IRequestHandler<AskKnowledgeQuery, AskKnowledgeResponse>
    {
        private readonly IHybridKnowledgeRetriever _hybridKnowledgeRetriever;
        private readonly IChatGenerator _chatGenerator;
        private readonly KnowledgeOptions _options;

        public AskKnowledgeHandler(IHybridKnowledgeRetriever hybridKnowledgeRetriever,
            IChatGenerator chatGenerator, IOptions<KnowledgeOptions> options)
        {
            _hybridKnowledgeRetriever = hybridKnowledgeRetriever;
            _chatGenerator = chatGenerator;
            _options = options.Value;
        }

        public async Task<AskKnowledgeResponse> Handle(
    AskKnowledgeQuery request,
    CancellationToken cancellationToken)
        {
            var results = await _hybridKnowledgeRetriever.RetrieveAsync(request.Question, sourceLimit: 10, resultLimit: 20, cancellationToken);

            var relevantChunks = results
                .Where(x => x.FinalScore >= _options.SimilarityThreshold)
                .Take(_options.ContextLimit)
                .ToList();

            if (relevantChunks.Count == 0)
            {
                return new AskKnowledgeResponse(request.Question, "No se encontró información suficiente en los documentos para responder la pregunta.");
            }

            var context = string.Join(
                Environment.NewLine + Environment.NewLine,
                relevantChunks.Select((x, index) =>
                $"""
                [Fragmento {index + 1}]
                {x.Content}
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

            var answer = await _chatGenerator.GenerateAsync(
                prompt,
                cancellationToken);

            return new AskKnowledgeResponse(request.Question, answer);

            //var contextChunks = relevantChunks
            //    .Select(chunk => new AskKnowledgeContextChunk(
            //        chunk.ChunkIndex,
            //        chunk.SemanticScore,
            //        chunk.TextScore,
            //        chunk.FinalScore,
            //        chunk.Content))
            //    .ToList();

            //return new AskKnowledgeResponse(
            //    request.Question,
            //    answer,
            //    contextChunks);
        }
    }
}
