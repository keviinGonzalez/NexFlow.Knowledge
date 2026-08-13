using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Search
{
    public sealed class SearchTermExtractor : ISearchTermExtractor
    {
        private static readonly HashSet<string> StopWords =
            new(StringComparer.OrdinalIgnoreCase)
            {
            "el",
            "la",
            "los",
            "las",
            "un",
            "una",
            "unos",
            "unas",
            "de",
            "del",
            "por",
            "para",
            "con",
            "sin",
            "que",
            "cual",
            "cuál",
            "es",
            "son",
            "se",
            "a",
            "en",
            "y",
            "o",
            "al"
            };

        public IReadOnlyList<string> Extract(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return [];

            return question
                .Split(
                    [' ', ',', '.', ';', ':', '?', '¿', '!', '¡'],
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length >= 3)
                .Where(x => !StopWords.Contains(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
