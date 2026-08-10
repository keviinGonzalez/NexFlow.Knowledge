using NexFlow.Knowledge.Application.Abstractions.Chunking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NexFlow.Knowledge.Infrastructure.Chunking
{
    public sealed class TextChunker : ITextChunker
    {
        private const int MaxChunkSize = 1000;
        private const int MinChunkSize = 300;
        private const int ChunkOverlap = 100;

        private static readonly Regex ArticleRegex = new(
            @"(?=\bART[IÍ]CULO\s+\d+[°º]?\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ParagraphRegex = new(
            @"(?=\bPAR[AÁ]GRAFO\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex NumeralRegex = new(
            @"(?=\b[A-Z]\.\d+[A-Z]?\.?\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IReadOnlyList<string> Chunk(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [];

            text = NormalizeText(text);
            var sections = SplitByArticles(text);

            var chunks = new List<string>();

            foreach (var section in sections)
            {
                if (string.IsNullOrWhiteSpace(section))
                    continue;

                var sectionChunks = ProcessSection(section);

                chunks.AddRange(sectionChunks);
            }

            return chunks;
        }

        private static List<string> ProcessSection(string section)
        {
            if (section.Length <= MaxChunkSize)
                return [section.Trim()];

            var parts = SplitByLegalStructure(section);

            var chunks = new List<string>();
            var current = new StringBuilder();

            foreach (var part in parts)
            {
                var normalizedPart = part.Trim();

                if (string.IsNullOrWhiteSpace(normalizedPart))
                    continue;

                // Si agregar la siguiente unidad supera el tamaño,
                // cerramos el chunk actual.
                if (current.Length > 0 &&
                    current.Length + normalizedPart.Length + 1 > MaxChunkSize)
                {
                    AddChunk(chunks, current.ToString());

                    current.Clear();

                    // Overlap controlado.
                    var overlap = GetOverlap(currentText: chunks[^1]);

                    if (!string.IsNullOrWhiteSpace(overlap))
                        current.Append(overlap);
                }

                // Si una unidad legal individual ya supera el límite,
                // la dividimos por estructura textual.
                if (normalizedPart.Length > MaxChunkSize)
                {
                    if (current.Length > 0)
                    {
                        AddChunk(chunks, current.ToString());
                        current.Clear();
                    }

                    var subChunks = SplitLargePart(normalizedPart);

                    foreach (var subChunk in subChunks)
                        chunks.Add(subChunk);

                    continue;
                }

                if (current.Length > 0)
                    current.Append(' ');

                current.Append(normalizedPart);
            }

            if (current.Length > 0)
                AddChunk(chunks, current.ToString());

            return chunks;
        }

        private static List<string> SplitByArticles(string text)
        {
            var matches = ArticleRegex.Matches(text);

            if (matches.Count == 0)
                return [text];

            var sections = new List<string>();

            // Texto previo al primer artículo.
            if (matches[0].Index > 0)
            {
                var preamble = text[..matches[0].Index].Trim();

                if (!string.IsNullOrWhiteSpace(preamble))
                    sections.Add(preamble);
            }

            for (var i = 0; i < matches.Count; i++)
            {
                var start = matches[i].Index;

                var end = i + 1 < matches.Count
                    ? matches[i + 1].Index
                    : text.Length;

                var section = text[start..end].Trim();

                if (!string.IsNullOrWhiteSpace(section))
                    sections.Add(section);
            }

            return sections;
        }

        private static List<string> SplitByLegalStructure(string text)
        {
            var parts = new List<string>();

            var matches = new List<Match>();

            matches.AddRange(ParagraphRegex.Matches(text));
            matches.AddRange(NumeralRegex.Matches(text));

            matches = matches
                .OrderBy(x => x.Index)
                .ToList();

            if (matches.Count == 0)
                return SplitByParagraphs(text);

            var currentPosition = 0;

            foreach (var match in matches)
            {
                if (match.Index > currentPosition)
                {
                    var previous = text[currentPosition..match.Index].Trim();

                    if (!string.IsNullOrWhiteSpace(previous))
                        parts.Add(previous);
                }

                currentPosition = match.Index;
            }

            if (currentPosition < text.Length)
            {
                var remaining = text[currentPosition..].Trim();

                if (!string.IsNullOrWhiteSpace(remaining))
                    parts.Add(remaining);
            }

            return parts;
        }

        private static List<string> SplitByParagraphs(string text)
        {
            var paragraphs = Regex.Split(text, @"\r?\n\s*\r?\n")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (paragraphs.Count > 1)
                return paragraphs;

            return SplitBySentences(text);
        }

        private static List<string> SplitBySentences(string text)
        {
            var sentences = Regex.Split(
                    text,
                    @"(?<=[.!?])\s+")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            return sentences;
        }

        private static List<string> SplitLargePart(string text)
        {
            var sentences = SplitBySentences(text);

            var chunks = new List<string>();
            var current = new StringBuilder();

            foreach (var sentence in sentences)
            {
                if (current.Length > 0 &&
                    current.Length + sentence.Length + 1 > MaxChunkSize)
                {
                    AddChunk(chunks, current.ToString());
                    current.Clear();
                }

                if (current.Length > 0)
                    current.Append(' ');

                current.Append(sentence);
            }

            if (current.Length > 0)
                AddChunk(chunks, current.ToString());

            return chunks;
        }

        private static void AddChunk(
            ICollection<string> chunks,
            string text)
        {
            var normalized = text.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
                return;

            chunks.Add(normalized);
        }

        private static string GetOverlap(string currentText)
        {
            if (string.IsNullOrWhiteSpace(currentText))
                return string.Empty;

            if (currentText.Length <= ChunkOverlap)
                return currentText;

            var start = currentText.Length - ChunkOverlap;

            var overlap = currentText[start..];

            var firstSpace = overlap.IndexOf(' ');

            if (firstSpace >= 0 && firstSpace < overlap.Length - 1)
                overlap = overlap[(firstSpace + 1)..];

            return overlap.Trim();
        }

        private static string NormalizeText(string text)
        {
            text = text.Replace("\r\n", "\n");

            text = Regex.Replace(
                text,
                @"[ \t]+",
                " ");

            text = Regex.Replace(
                text,
                @"\n{3,}",
                "\n\n");

            return text.Trim();
        }
    }
}
