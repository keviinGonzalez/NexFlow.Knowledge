using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Domain.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected init; } = Guid.NewGuid();
    }
}
