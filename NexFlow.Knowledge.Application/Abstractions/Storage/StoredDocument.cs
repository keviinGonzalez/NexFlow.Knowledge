using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Application.Abstractions.Storage
{
    public sealed record StoredDocument(string StoredFileName, string StoragePath);
}
