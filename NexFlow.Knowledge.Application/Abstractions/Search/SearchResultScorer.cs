using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Search
{
    public sealed class SearchResultScorer : ISearchResultScorer
    {
        public double Calculate(double semanticScore, double textScore)
        {
            return (semanticScore * 0.7) + (textScore * 0.3);
        }
    }
}
