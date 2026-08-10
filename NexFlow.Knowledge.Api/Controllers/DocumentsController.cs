using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexFlow.Knowledge.Api.Extensions;
using NexFlow.Knowledge.Application.Features.Documents.Commands.UploadDocument;

namespace NexFlow.Knowledge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class DocumentsController : ControllerBase
    {
        private readonly ISender _sender;
        public DocumentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UploadDocumentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            var command = await file.ToUploadDocumentCommandAsync(cancellationToken);
            var response = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            return Ok();
        }
    }
}
