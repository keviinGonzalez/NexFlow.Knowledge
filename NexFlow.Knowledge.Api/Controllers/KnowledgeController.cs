using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexFlow.Knowledge.Application.Features.Knowledge.Queries.AskKnowledge;
using NexFlow.Knowledge.Application.Features.Knowledge.Queries.SearchKnowledge;

namespace NexFlow.Knowledge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeController : ControllerBase
    {
        private readonly ISender _sender;

        public KnowledgeController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("search")]
        public async Task<ActionResult<SearchKnowledgeResponse>> Search([FromBody] SearchKnowledgeQuery query, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpPost("ask")]
        public async Task<ActionResult<AskKnowledgeResponse>> Ask([FromBody] AskKnowledgeQuery query, CancellationToken cancellationToken)
        {
            var response = await _sender.Send(query, cancellationToken);
            return Ok(response);
        }
    }
}
