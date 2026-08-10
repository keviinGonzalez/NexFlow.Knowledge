using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Application.Abstractions.Parsing;
using NexFlow.Knowledge.Infrastructure.Parsing;

namespace NexFlow.Knowledge.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ChatTestController : ControllerBase
    {
        private readonly IChatGenerator _chatGenerator;
        private readonly ITextNormalizer _textNormalizer;

        public ChatTestController(IChatGenerator chatGenerator, ITextNormalizer textNormalizer)
        {
            _chatGenerator = chatGenerator;
            _textNormalizer = textNormalizer;
        }

        [HttpPost]
        public async Task<IActionResult> Generate(
            [FromBody] string prompt,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return BadRequest("El prompt es obligatorio.");

            var response = await _chatGenerator.GenerateAsync(
                prompt,
                cancellationToken);

            return Ok(new
            {
                response
            });
        }

        [HttpPost("test-chunking")]
        public async Task<IActionResult> TestChunking(
 IFormFile file,
 [FromServices] IDocumentParser parser,
 [FromServices] ITextChunker chunker,
 CancellationToken cancellationToken)
        {
            await using var stream = file.OpenReadStream();

var extractedText = await parser.ExtractTextAsync(
    stream,
    cancellationToken);

            var normalizedText = _textNormalizer.Normalize(extractedText);

            var chunks = chunker.Chunk(normalizedText);

            return Ok(new
            {
                extractedCharacters = extractedText.Length,
                normalizedCharacters = normalizedText.Length,
                totalChunks = chunks.Count,
                chunks
            });

        }

    }
}
