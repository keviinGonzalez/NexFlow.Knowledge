using NexFlow.Knowledge.Domain.Entities;

namespace NexFlow.Knowledge.Domain.Repositories.Search
{
    public sealed record TextSearchResult(DocumentChunk Chunk, double TextScore);
}
