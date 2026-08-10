using Microsoft.Extensions.Options;
using NexFlow.Knowledge.Application.Abstractions.Storage;
using NexFlow.Knowledge.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Storage
{
    public class LocalDocumentStorage : IDocumentStorage
    {
        private readonly DocumentStorageOptions _options;

        public LocalDocumentStorage(IOptions<DocumentStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<StoredDocument> SaveAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default)
        {
            var ext = Path.GetExtension(originalFileName);
            var storedFileName = $"{Guid.NewGuid()}{ext}";
            var directory = Path.Combine(Directory.GetCurrentDirectory(), _options.Path);
            Directory.CreateDirectory(directory);
            var fullPath = Path.Combine(directory, storedFileName);
            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await content.CopyToAsync(fileStream, cancellationToken);
            return new StoredDocument(storedFileName, fullPath);
        }
    }
}
