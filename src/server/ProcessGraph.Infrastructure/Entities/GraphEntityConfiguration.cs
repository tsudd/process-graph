using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Graphs;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class GraphEntityConfiguration : IEntityTypeConfiguration<Graph>
{
    public void Configure(EntityTypeBuilder<Graph> builder)
    {
        builder.HasKey(g => g.Id);
        
        builder.Property<uint>("Version").IsRowVersion();
    }
}
