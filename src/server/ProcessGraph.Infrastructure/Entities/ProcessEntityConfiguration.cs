using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class ProcessEntityConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.ToTable("processes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(p => p.LastModifiedAt)
            .HasColumnType("timestamptz");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_processes_name")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder.Property(p => p.Description).HasMaxLength(300);

        builder.Property(p => p.Status);

        builder.OwnsOne(p => p.Settings, settingsBuilder =>
        {
            settingsBuilder.Property(s => s.Unit)
                .HasConversion(
                    v => v.ToString(),
                    v => UnitOfMeasure.FromName(v))
                .IsRequired();
        });

        builder.Property(p => p.Graph).HasColumnType("jsonb");
        
        builder.Property<uint>("Version").IsRowVersion();
    }
}