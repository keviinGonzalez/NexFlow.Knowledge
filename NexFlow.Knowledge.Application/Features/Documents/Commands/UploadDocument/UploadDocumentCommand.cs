using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Documents.Commands.UploadDocument
{
    public sealed record UploadDocumentCommand(byte[] Content, string FileName, string ContentType, long FileSize) : IRequest<UploadDocumentResponse>;
}
