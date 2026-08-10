using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Documents.Commands.UploadDocument
{
    public sealed record UploadDocumentResponse(Guid Id, string FileName, DateTime UploadedAt);
}
