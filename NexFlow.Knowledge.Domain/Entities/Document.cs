using NexFlow.Knowledge.Domain.Base;
using NexFlow.Knowledge.Domain.Exceptions;
using NexFlow.Knowledge.Domain.Guards;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Entities
{
    public sealed class Document : BaseEntity
    {
        public string OriginalFileName { get; private set; }
        public string StoredFileName { get; private set; }
        public string StoragePath { get; private set; }
        public string ContentType { get; private set; }
        public long FileSize { get; private set; }
        public string? ExtractedText { get; private set; }
        public DateTime UploadedAt { get; private set; }
        private Document()
        {
        }

        public static Document Create(string originalFileName, string storedFileName, string storagePath, string contentType,
    long fileSize, string? extractedText = null)
        {
            var document = new Document();

            document.ChangeOriginalFileName(originalFileName);
            document.ChangeStoredFileName(storedFileName);
            document.ChangeStoragePath(storagePath);
            document.ChangeContentType(contentType);
            document.ChangeFileSize(fileSize);
            document.ChangeExtractedText(extractedText);

            document.UploadedAt = DateTime.UtcNow;

            return document;
        }

        public void ChangeOriginalFileName(string originalFileName)
        {
            Guard.ValidateRequired(originalFileName, nameof(originalFileName));
            Guard.ValidateMaxLength(originalFileName, 255, nameof(originalFileName));

            OriginalFileName = originalFileName.Trim();
        }

        public void ChangeStoredFileName(string storedFileName)
        {
            Guard.ValidateRequired(storedFileName, nameof(storedFileName));
            Guard.ValidateMaxLength(storedFileName, 255, nameof(storedFileName));

            StoredFileName = storedFileName.Trim();
        }

        public void ChangeStoragePath(string storagePath)
        {
            Guard.ValidateRequired(storagePath, nameof(storagePath));
            Guard.ValidateMaxLength(storagePath, 500, nameof(storagePath));

            StoragePath = storagePath.Trim();
        }

        public void ChangeContentType(string contentType)
        {
            Guard.ValidateRequired(contentType, nameof(contentType));
            Guard.ValidateMaxLength(contentType, 100, nameof(contentType));

            ContentType = contentType.Trim();
        }

        public void ChangeFileSize(long fileSize)
        {
            if (fileSize <= 0)
            {
                throw new DomainException("File size must be greater than zero.");
            }

            FileSize = fileSize;
        }

        public void ChangeExtractedText(string? extractedText)
        {
            if (string.IsNullOrWhiteSpace(extractedText))
            {
                ExtractedText = null;
                return;
            }

            ExtractedText = extractedText.Trim();
        }
    }
}
