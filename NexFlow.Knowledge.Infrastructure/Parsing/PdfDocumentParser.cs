using NexFlow.Knowledge.Application.Abstractions.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace NexFlow.Knowledge.Infrastructure.Parsing
{
    public sealed class PdfDocumentParser : IDocumentParser
    {
        private const double LineTolerance = 3.0;


        public Task<string> ExtractTextAsync(
            Stream content,
            CancellationToken cancellationToken = default)
        {
            using var document = PdfDocument.Open(content);

            var pages = new List<string>();

            foreach (var page in document.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var words = page
                    .GetWords()
                    .OrderByDescending(w => w.BoundingBox.Top)
                    .ThenBy(w => w.BoundingBox.Left)
                    .ToList();

                var lines = GroupIntoLines(words);

                var pageText = string.Join(
                    Environment.NewLine,
                    lines.Select(BuildLine));

                pages.Add(pageText);
            }

            return Task.FromResult(
                string.Join(
                    Environment.NewLine + Environment.NewLine,
                    pages));
        }

        private static List<List<Word>> GroupIntoLines(
            IReadOnlyList<Word> words)
        {
            var lines = new List<List<Word>>();

            foreach (var word in words)
            {
                var line = lines.FirstOrDefault(existingLine =>
                    Math.Abs(
                        existingLine[0].BoundingBox.Top -
                        word.BoundingBox.Top) <= LineTolerance);

                if (line is null)
                {
                    lines.Add([word]);
                    continue;
                }

                line.Add(word);
            }

            foreach (var line in lines)
            {
                line.Sort(
                    (a, b) => a.BoundingBox.Left.CompareTo(
                        b.BoundingBox.Left));
            }

            return lines;
        }

        private static string BuildLine(
            IReadOnlyList<Word> words)
        {
            if (words.Count == 0)
                return string.Empty;

            var result = words[0].Text;

            for (var i = 1; i < words.Count; i++)
            {
                var previous = words[i - 1];
                var current = words[i];

                var gap = current.BoundingBox.Left -
                          previous.BoundingBox.Right;

                // Un espacio normal entre palabras.
                // Si están prácticamente pegadas, también
                // dejamos espacio porque PdfPig ya entregó
                // cada elemento como una palabra.
                result += " ";
                result += current.Text;
            }

            return result.Trim();
        }


    }

}
