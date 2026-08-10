using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.SearchKnowledge
{
    public sealed record SearchKnowledgeQuery(string Question) : IRequest<SearchKnowledgeResponse>;
}
