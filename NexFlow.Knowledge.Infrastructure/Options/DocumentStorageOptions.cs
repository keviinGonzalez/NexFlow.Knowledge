using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Options
{
    public sealed class DocumentStorageOptions
    {
        public const string SectionName = "DocumentStorage";
        public string Path { get; set; } = "storage/documents";
    }
}
