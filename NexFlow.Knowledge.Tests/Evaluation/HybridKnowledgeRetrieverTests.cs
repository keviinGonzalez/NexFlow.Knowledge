using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Search;
using NexFlow.Knowledge.Domain.Entities;
using NexFlow.Knowledge.Domain.Repositories;
using NexFlow.Knowledge.Domain.Repositories.Search;
using System;
using System.Collections.Generic;
using Pgvector;

using System.Text;
using System.Timers;
using Moq;
using NexFlow.Knowledge.Application.Services.Search;

namespace NexFlow.Knowledge.Tests.Evaluation
{
    public sealed class HybridKnowledgeRetrieverTest
    {
        [Fact]
        public async Task RetrieveAsync_ShouldMergeSemanticAndTextResults_ForSameChunk()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var chunk = new DocumentChunk(
                documentId,
                "El sistema no permite reutilizar las últimas cinco contraseñas utilizadas por el usuario.",
                6);

            var embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f });

            var embeddingGenerator = new Mock<IEmbeddingGenerator>();
            var repository = new Mock<IDocumentChunkRepository>();
            var termExtractor = new Mock<ISearchTermExtractor>();
            var scorer = new Mock<ISearchResultScorer>();

            embeddingGenerator
                .Setup(x => x.GenerateAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(embedding);

            termExtractor
                .Setup(x => x.Extract(It.IsAny<string>()))
                .Returns(
                [
                    "contraseñas",
                "reutilizarse"
                ]);

            repository
                .Setup(x => x.SearchSimilarAsync(
                    It.IsAny<Vector>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new SimilarChunkResult(
                    chunk,
                    0.70)
                ]);

            repository
                .Setup(x => x.SearchTextAsync(
                    It.IsAny<IReadOnlyList<string>>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new TextSearchResult(
                    chunk,
                    0.30)
                ]);

            scorer
                .Setup(x => x.Calculate(0.70, 0.30))
                .Returns(0.50);

            var retriever = new HybridKnowledgeRetriever(
                embeddingGenerator.Object,
                repository.Object,
                termExtractor.Object,
                scorer.Object);

            // Act
            var results = await retriever.RetrieveAsync(
                "¿Cuántas contraseñas anteriores no pueden reutilizarse?",
                sourceLimit: 10,
                resultLimit: 20);

            // Assert
            var result = Assert.Single(results);

            Assert.Equal(documentId, result.DocumentId);
            Assert.Equal(6, result.ChunkIndex);

            Assert.Equal(0.70, result.SemanticScore);
            Assert.Equal(0.30, result.TextScore);
            Assert.Equal(0.50, result.FinalScore);

            scorer.Verify(
                x => x.Calculate(0.70, 0.30),
                Times.Once);
        }

        [Fact]
        public async Task RetrieveAsync_ShouldOrderResultsByFinalScoreDescending()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var chunk1 = new DocumentChunk(
                documentId,
                "Contenido del primer fragmento.",
                1);

            var chunk2 = new DocumentChunk(
                documentId,
                "Contenido del segundo fragmento.",
                2);

            var embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f });

            var embeddingGenerator = new Mock<IEmbeddingGenerator>();
            var repository = new Mock<IDocumentChunkRepository>();
            var termExtractor = new Mock<ISearchTermExtractor>();
            var scorer = new Mock<ISearchResultScorer>();

            embeddingGenerator
                .Setup(x => x.GenerateAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(embedding);

            termExtractor
                .Setup(x => x.Extract(It.IsAny<string>()))
                .Returns(["primer", "segundo"]);

            repository
                .Setup(x => x.SearchSimilarAsync(
                    It.IsAny<Vector>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new SimilarChunkResult(chunk1, 0.60),
            new SimilarChunkResult(chunk2, 0.90)
                ]);

            repository
                .Setup(x => x.SearchTextAsync(
                    It.IsAny<IReadOnlyList<string>>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new TextSearchResult(chunk1, 0.20),
            new TextSearchResult(chunk2, 0.30)
                ]);

            scorer
                .Setup(x => x.Calculate(0.60, 0.20))
                .Returns(0.40);

            scorer
                .Setup(x => x.Calculate(0.90, 0.30))
                .Returns(0.80);

            var retriever = new HybridKnowledgeRetriever(
                embeddingGenerator.Object,
                repository.Object,
                termExtractor.Object,
                scorer.Object);

            // Act
            var results = await retriever.RetrieveAsync(
                "pregunta de prueba",
                sourceLimit: 10,
                resultLimit: 20);

            // Assert
            Assert.Equal(2, results.Count);

            Assert.Equal(2, results[0].ChunkIndex);
            Assert.Equal(0.80, results[0].FinalScore);

            Assert.Equal(1, results[1].ChunkIndex);
            Assert.Equal(0.40, results[1].FinalScore);
        }

        [Fact]
        public async Task RetrieveAsync_ShouldRespectResultLimit()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var chunk1 = new DocumentChunk(
                documentId,
                "Contenido uno.",
                1);

            var chunk2 = new DocumentChunk(
                documentId,
                "Contenido dos.",
                2);

            var chunk3 = new DocumentChunk(
                documentId,
                "Contenido tres.",
                3);

            var embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f });

            var embeddingGenerator = new Mock<IEmbeddingGenerator>();
            var repository = new Mock<IDocumentChunkRepository>();
            var termExtractor = new Mock<ISearchTermExtractor>();
            var scorer = new Mock<ISearchResultScorer>();

            embeddingGenerator
                .Setup(x => x.GenerateAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(embedding);

            termExtractor
                .Setup(x => x.Extract(It.IsAny<string>()))
                .Returns(["contenido"]);

            repository
                .Setup(x => x.SearchSimilarAsync(
                    It.IsAny<Vector>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new SimilarChunkResult(chunk1, 0.90),
            new SimilarChunkResult(chunk2, 0.80),
            new SimilarChunkResult(chunk3, 0.70)
                ]);

            repository
                .Setup(x => x.SearchTextAsync(
                    It.IsAny<IReadOnlyList<string>>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            scorer
                .Setup(x => x.Calculate(It.IsAny<double>(), It.IsAny<double>()))
                .Returns((double semantic, double text) => semantic);

            var retriever = new HybridKnowledgeRetriever(
                embeddingGenerator.Object,
                repository.Object,
                termExtractor.Object,
                scorer.Object);

            // Act
            var results = await retriever.RetrieveAsync(
                "pregunta de prueba",
                sourceLimit: 10,
                resultLimit: 2);

            // Assert
            Assert.Equal(2, results.Count);

            Assert.Equal(1, results[0].ChunkIndex);
            Assert.Equal(2, results[1].ChunkIndex);
        }

        [Fact]
        public async Task RetrieveAsync_ShouldKeepCandidatesFoundByOnlyOneSearchStrategy()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            var semanticChunk = new DocumentChunk(
                documentId,
                "Encontrado mediante búsqueda semántica.",
                1);

            var textChunk = new DocumentChunk(
                documentId,
                "Encontrado mediante búsqueda textual.",
                2);

            var embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f });

            var embeddingGenerator = new Mock<IEmbeddingGenerator>();
            var repository = new Mock<IDocumentChunkRepository>();
            var termExtractor = new Mock<ISearchTermExtractor>();
            var scorer = new Mock<ISearchResultScorer>();

            embeddingGenerator
                .Setup(x => x.GenerateAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(embedding);

            termExtractor
                .Setup(x => x.Extract(It.IsAny<string>()))
                .Returns(["búsqueda"]);

            repository
                .Setup(x => x.SearchSimilarAsync(
                    It.IsAny<Vector>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new SimilarChunkResult(semanticChunk, 0.80)
                ]);

            repository
                .Setup(x => x.SearchTextAsync(
                    It.IsAny<IReadOnlyList<string>>(),
                    10,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new TextSearchResult(textChunk, 0.60)
                ]);

            scorer
                .Setup(x => x.Calculate(0.80, 0))
                .Returns(0.80);

            scorer
                .Setup(x => x.Calculate(0, 0.60))
                .Returns(0.60);

            var retriever = new HybridKnowledgeRetriever(
                embeddingGenerator.Object,
                repository.Object,
                termExtractor.Object,
                scorer.Object);

            // Act
            var results = await retriever.RetrieveAsync(
                "pregunta de prueba",
                sourceLimit: 10,
                resultLimit: 20);

            // Assert
            Assert.Equal(2, results.Count);

            var semanticResult = results.Single(x => x.ChunkIndex == 1);
            var textResult = results.Single(x => x.ChunkIndex == 2);

            Assert.Equal(0.80, semanticResult.SemanticScore);
            Assert.Equal(0, semanticResult.TextScore);

            Assert.Equal(0, textResult.SemanticScore);
            Assert.Equal(0.60, textResult.TextScore);
        }
    }
}
