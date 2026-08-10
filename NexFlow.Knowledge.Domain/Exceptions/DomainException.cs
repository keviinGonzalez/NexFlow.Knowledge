using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
