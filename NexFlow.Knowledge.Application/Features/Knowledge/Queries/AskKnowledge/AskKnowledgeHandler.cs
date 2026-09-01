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
                Eres un asistente virtual experto y empático. Tu objetivo es responder la duda del usuario de forma clara, directa y con un tono natural y conversacional.

                Instrucciones clave:
                1. Basa tu respuesta exclusivamente en la información provista en la sección "Contexto".
                2. Sintetiza y redacta la respuesta con tus propias palabras; no te limites a copiar y pegar fragmentos salvo que sea estrictamente necesario (como artículos o citas exactas).
                3. Si la información del contexto no es suficiente para responder con certeza, sé honesto y menciona con naturalidad que no cuentas con esos datos en tu base de conocimientos, sin especular.
                4. Responde directamente a lo que se pregunta, omitiendo introducciones robóticas como "Según el contexto provisto...".

                ---
                Contexto:
                {context}
                ---

                Pregunta del usuario:
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
