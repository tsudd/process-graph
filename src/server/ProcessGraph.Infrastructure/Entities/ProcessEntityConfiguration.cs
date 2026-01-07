using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class ProcessEntityConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.HasKey(process => process.Id);

        builder.Property(process => process.Status)
            .HasConversion<int>();

        builder.OwnsOne(process => process.Settings, settingsBuilder =>
        {
            settingsBuilder.Property(s => s.Unit)
                .HasConversion(unit => unit.ToString(), s => UnitOfMeasure.FromName(s));
        });
    }
}
