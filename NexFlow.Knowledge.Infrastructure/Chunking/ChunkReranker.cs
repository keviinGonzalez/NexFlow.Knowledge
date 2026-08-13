using NexFlow.Knowledge.Application.Abstractions.Chunking;
using NexFlow.Knowledge.Domain.Repositories.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Chunking
{
    public sealed class ChunkReranker : IChunkReranker
    {
        public IReadOnlyList<SimilarChunkResult> Rerank(
            string question,
            IReadOnlyList<SimilarChunkResult> candidates)
        {
            if (string.IsNullOrWhiteSpace(question))
                return candidates;

            if (candidates.Count == 0)
                return candidates;

            var questionTerms = ExtractTerms(question);

            return candidates
                .Select(candidate =>
                {
                    var textScore = CalculateTextScore(
                        candidate.Chunk.Content,
                        questionTerms);

                    var finalScore =
                        (candidate.Similarity * 0.70) +
                        (textScore * 0.30);

                    return new RankedChunk(
                        candidate,
                        finalScore,
                        textScore);
                })
                .OrderByDescending(x => x.FinalScore)
                .Select(x => x.Result)
                .ToList();
        }

        private static HashSet<string> ExtractTerms(string text)
        {
            return text
                .ToLowerInvariant()
                .Split(
                    [
                        ' ',
                    '\n',
                    '\r',
                    '\t',
                    '.',
                    ',',
                    ';',
                    ':',
                    '?',
                    '!',
                    '(',
                    ')',
                    '"',
                    '\'',
                    '¿',
                    '¡'
                    ],
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length >= 3)
                .Where(x => !IsStopWord(x))
                .ToHashSet();
        }

        private static double CalculateTextScore(
            string content,
            HashSet<string> questionTerms)
        {
            if (string.IsNullOrWhiteSpace(content) ||
                questionTerms.Count == 0)
            {
                return 0;
            }

            var contentTerms = ExtractTerms(content);

            var matches = questionTerms.Count(contentTerms.Contains);

            return (double)matches / questionTerms.Count;
        }

        private static bool IsStopWord(string word)
        {
            return word switch
            {
                "que" => true,
                "cuál" => true,
                "cual" => true,
                "por" => true,
                "para" => true,
                "una" => true,
                "uno" => true,
                "los" => true,
                "las" => true,
                "del" => true,
                "con" => true,
                "como" => true,
                _ => false
            };
        }

        private sealed record RankedChunk(
            SimilarChunkResult Result,
            double FinalScore,
            double TextScore);
    }


}
