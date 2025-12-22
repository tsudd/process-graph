using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Graphs;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class ProcessEntityConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.ToTable("processes");

        builder.HasKey(process => process.Id);

        builder.Property(process => process.Name).HasMaxLength(200);
        builder.Property(process => process.Description).HasMaxLength(500);
        builder.Property(process => process.Status).HasConversion<int>().HasMaxLength(50);
        builder.Property(process => process.CreatedAt).HasDefaultValueSql("getdate()");
        builder.Property(process => process.LastModifiedAt);
        builder.OwnsOne(process => process.Settings,
            settingsBuilder =>
            {
                settingsBuilder.Property(s => s.Unit)
                    .HasConversion(unit => unit.ToString(), s => UnitOfMeasure.FromName(s));
            });
        
        builder.HasOne(process => process.Graph)
            .WithOne()
            .HasForeignKey<Graph>(graph => graph.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}