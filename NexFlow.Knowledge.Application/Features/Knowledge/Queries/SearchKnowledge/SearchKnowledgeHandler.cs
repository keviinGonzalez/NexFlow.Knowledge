using MediatR;
using NexFlow.Knowledge.Application.Abstractions.Search;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.SearchKnowledge
{
    public sealed class SearchKnowledgeHandler : IRequestHandler<SearchKnowledgeQuery, SearchKnowledgeResponse>
    {
        private readonly IHybridKnowledgeRetriever _hybridKnowledgeRetriever;

        public SearchKnowledgeHandler(
            IHybridKnowledgeRetriever hybridKnowledgeRetriever)
        {
            _hybridKnowledgeRetriever = hybridKnowledgeRetriever;
        }

        public async Task<SearchKnowledgeResponse> Handle(
            SearchKnowledgeQuery request,
            CancellationToken cancellationToken)
        {
            var results = await _hybridKnowledgeRetriever.RetrieveAsync(
                    request.Question,
                    sourceLimit: 10,
                    resultLimit: 20,
                    cancellationToken);               

            var responseResults = results
                .Select(result => new SearchKnowledgeResult(
                    result.DocumentId,
                    result.ChunkIndex,
                    result.Content,
                    result.SemanticScore,
                    result.TextScore,
                    result.FinalScore))
                .ToList();

            return new SearchKnowledgeResponse(
                request.Question,
                responseResults);
        }


    }
}
