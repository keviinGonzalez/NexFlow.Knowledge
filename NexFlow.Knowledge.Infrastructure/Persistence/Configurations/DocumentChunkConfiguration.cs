using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexFlow.Knowledge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexFlow.Knowledge.Infrastructure.Persistence.Configurations
{
    internal class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
    {
        public void Configure(EntityTypeBuilder<DocumentChunk> builder)
        {
            builder.ToTable("DocumentChunks");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.DocumentId).IsRequired();
            builder.Property(x => x.Content).HasColumnType("text").IsRequired();
            builder.Property(x => x.ChunkIndex).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.Embedding).HasColumnType("vector(768)").IsRequired(false);

            builder.HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(x => x.DocumentId);
            builder.HasIndex(x => new { x.DocumentId, x.ChunkIndex }).IsUnique();
        }
    }
}
