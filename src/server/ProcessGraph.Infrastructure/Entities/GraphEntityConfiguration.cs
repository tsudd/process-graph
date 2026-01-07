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
        
        builder.OwnsMany(g => g.Nodes, nodeBuilder =>
        {
            nodeBuilder.HasKey(n => n.Id);
            nodeBuilder.Property(n => n.Label).IsRequired();
            nodeBuilder.Property(n => n.XPosition).IsRequired();
            nodeBuilder.Property(n => n.YPosition).IsRequired();
            nodeBuilder.Property(n => n.Description);
        });
        
        builder.OwnsMany(g => g.Edges, edgeBuilder =>
        {
            edgeBuilder.HasKey(e => new { e.From, e.To });
            edgeBuilder.Property(e => e.From).IsRequired();
            edgeBuilder.Property(e => e.To).IsRequired();
            edgeBuilder.Property(e => e.Description);
            edgeBuilder.Property(e => e.Label);
            edgeBuilder.Property(e => e.Value);
        });
    }
}
