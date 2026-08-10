using System;
using System.Collections.Generic;

namespace NexFlow.Knowledge.Application.Abstractions.Options
{
    public sealed class KnowledgeOptions
    {
        public const string SectionName = "Knowledge";
        public int RetrievalLimit { get; set; } = 10;
        public int ContextLimit { get; set; } = 5;
        public double SimilarityThreshold { get; set; } = 0.70;
    }
}
