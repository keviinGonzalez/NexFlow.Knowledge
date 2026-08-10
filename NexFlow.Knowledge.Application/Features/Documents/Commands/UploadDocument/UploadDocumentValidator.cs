using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Documents.Commands.UploadDocument
{
    public sealed class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
    {
        public UploadDocumentValidator()
        {
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
            RuleFor(x => x.ContentType).Equal("application/pdf");
            RuleFor(x => x.FileSize).GreaterThan(0);
            RuleFor(x => x.Content).NotNull();
        }
    }
}
