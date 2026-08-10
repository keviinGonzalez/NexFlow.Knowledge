using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NexFlow.Knowledge.Application.Abstractions.AI;
using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Application.Abstractions.Parsing;
using NexFlow.Knowledge.Application.Abstractions.Persistence;
using NexFlow.Knowledge.Application.Abstractions.Storage;
using NexFlow.Knowledge.Domain.Repositories;
using NexFlow.Knowledge.Infrastructure.AI.Ollama;
using NexFlow.Knowledge.Infrastructure.AI.Options;
using NexFlow.Knowledge.Infrastructure.Chunking;
using NexFlow.Knowledge.Infrastructure.Options;
using NexFlow.Knowledge.Infrastructure.Parsing;
using NexFlow.Knowledge.Infrastructure.Persistence;
using NexFlow.Knowledge.Infrastructure.Persistence.Context;
using NexFlow.Knowledge.Infrastructure.Persistence.Repositories;
using NexFlow.Knowledge.Infrastructure.Persistence.Services;
using NexFlow.Knowledge.Infrastructure.Storage;

namespace NexFlow.Knowledge.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions =>
                    {
                        npgsqlOptions.UseVector();
                    }));

            services.AddHttpClient<IEmbeddingGenerator, OllamaEmbeddingGenerator>((serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<OllamaOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });

            services.AddHttpClient<IChatGenerator, OllamaChatGenerator>((serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<OllamaOptions>>()
                    .Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            });

            services.Configure<OllamaOptions>(configuration.GetSection(OllamaOptions.SectionName));
            services.Configure<DocumentStorageOptions>(configuration.GetSection(DocumentStorageOptions.SectionName));
            services.Configure<KnowledgeOptions>(configuration.GetSection(KnowledgeOptions.SectionName));


            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentStorage, LocalDocumentStorage>();
            services.AddScoped<IDocumentParser, PdfDocumentParser>();
            services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
            services.AddScoped<IDocumentChunkService, DocumentChunkService>();
            services.AddScoped<ITextChunker, TextChunker>();
            services.AddScoped<ITextNormalizer, TextNormalizer>();
            services.AddScoped<IChunkContextExpander, ChunkContextExpander>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
    }
}
