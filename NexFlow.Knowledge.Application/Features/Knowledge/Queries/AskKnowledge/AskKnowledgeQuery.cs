using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Knowledge.Queries.AskKnowledge
{
    public sealed record AskKnowledgeQuery(string Question) : IRequest<AskKnowledgeResponse>;
}
