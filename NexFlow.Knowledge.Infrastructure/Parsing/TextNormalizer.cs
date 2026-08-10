using NexFlow.Knowledge.Application.Abstractions.Parsing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NexFlow.Knowledge.Infrastructure.Parsing
{
    public class TextNormalizer : ITextNormalizer
    {
        public string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");

            // Une palabras que quedaron partidas por un salto de línea.
            // Ejemplo: "particu-\nlares" -> "particulares".
            text = Regex.Replace(
                text,
                @"(\p{L})-\n(\p{L})",
                "$1$2");

            // Convierte saltos de línea simples en espacios.
            // Conservamos los saltos dobles como separación de párrafos.
            text = Regex.Replace(
                text,
                @"(?<!\n)\n(?!\n)",
                " ");

            // Reduce espacios consecutivos.
            text = Regex.Replace(
                text,
                @"[ \t]+",
                " ");

            // Reduce más de dos saltos consecutivos.
            text = Regex.Replace(
                text,
                @"\n{3,}",
                "\n\n");

            return text.Trim();
        }
    }
}
