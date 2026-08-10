using NexFlow.Knowledge.Application.Features.Documents.Commands.UploadDocument;

namespace NexFlow.Knowledge.Api.Extensions
{
    public static class IFormFileExtensions
    {
        public static async Task<UploadDocumentCommand> ToUploadDocumentCommandAsync(this IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            return new UploadDocumentCommand(stream.ToArray(), file.FileName, file.ContentType, file.Length);
        }
    }
}
