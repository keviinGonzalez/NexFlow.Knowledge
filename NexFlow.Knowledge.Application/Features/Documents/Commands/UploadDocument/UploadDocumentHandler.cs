using MediatR;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Application.Abstractions.Parsing;
using NexFlow.Knowledge.Application.Abstractions.Persistence;
using NexFlow.Knowledge.Application.Abstractions.Storage;
using NexFlow.Knowledge.Domain.Entities;
using NexFlow.Knowledge.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Features.Documents.Commands.UploadDocument
{
    public sealed class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, UploadDocumentResponse>
    {
        private readonly IDocumentStorage _storage;
        private readonly IDocumentParser _parser;
        private readonly IDocumentRepository _repository;
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly IDocumentChunkService _documentChunkService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITextNormalizer _textNormalizer;

        public UploadDocumentHandler(IDocumentStorage storage, IDocumentParser parser, IDocumentRepository repository,
            IDocumentChunkRepository documentChunkRepository, IDocumentChunkService documentChunkService, IUnitOfWork unitOfWork, 
            ITextNormalizer textNormalizer)
        {
            _storage = storage;
            _parser = parser;
            _repository = repository;
            _documentChunkRepository = documentChunkRepository;
            _documentChunkService = documentChunkService;
            _unitOfWork = unitOfWork;
            _textNormalizer = textNormalizer;
        }
        public async Task<UploadDocumentResponse> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            using var storageStream = new MemoryStream(request.Content);
            using var parserStream = new MemoryStream(request.Content);

            var storedDocument = await _storage.SaveAsync(storageStream, request.FileName, cancellationToken);
            var extractedText = await _parser.ExtractTextAsync(parserStream, cancellationToken);
            var normalizedText = _textNormalizer.Normalize(extractedText);
            var document = Document.Create(request.FileName, storedDocument.StoredFileName, storedDocument.StoragePath,
                request.ContentType, request.FileSize, normalizedText);
            await _repository.AddAsync(document, cancellationToken);

            var documentChunks = await _documentChunkService.CreateChunksAsync(document, normalizedText, cancellationToken);
            await _documentChunkRepository.AddRangeAsync(documentChunks, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UploadDocumentResponse(document.Id, document.OriginalFileName, document.UploadedAt);
        }
    }
}
