using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexFlow.Knowledge.Domain.Entities;
using Pgvector;
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
            builder.Property(x => x.Embedding)
          .HasColumnType("vector(768)").IsRequired(false)
          // Agregamos la conversión automática de float[] <=> Pgvector.Vector
          .HasConversion(
              v => v != null ? new Vector(v) : null,      // Al guardar en BD
              v => v != null ? v.ToArray() : null        // Al leer de la BD
          );

            //builder.HasOne(x => x.Document).WithMany().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Document).WithMany(d => d.Chunks) // EF Core ahora sabe que Chunks mapea aquí
           .HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Cascade);

            // Le indicamos a EF Core que acceda a la colección a través del campo privado
            builder.Metadata.FindNavigation(nameof(Document.Chunks))?.SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.HasIndex(x => x.DocumentId);
            builder.HasIndex(x => new { x.DocumentId, x.ChunkIndex }).IsUnique();
        }
    }
}
