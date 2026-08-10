using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Persistence.Configurations
{
    public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
            builder.Property(x => x.StoredFileName).HasMaxLength(255).IsRequired();
            builder.Property(x => x.StoragePath).HasMaxLength(500).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.FileSize).IsRequired();
            builder.Property(x => x.ExtractedText).HasColumnType("text");
            builder.Property(x => x.UploadedAt).IsRequired();

            builder.HasIndex(x => x.StoredFileName).IsUnique();
        }
    }
}
