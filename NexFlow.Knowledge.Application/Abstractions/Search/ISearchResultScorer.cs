using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Search
{
    public interface ISearchResultScorer
    {
        double Calculate(double semanticScore, double textScore);
    }
}
