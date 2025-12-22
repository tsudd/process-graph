using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Graphs;
using System.Text.Json;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class GraphEntityConfiguration : IEntityTypeConfiguration<Graph>
{
    public void Configure(EntityTypeBuilder<Graph> builder)
    {
        builder.ToTable("graphs");
        
        builder.HasKey(g => g.Id);
        
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        
        builder.Property(g => g.Nodes)
            .HasColumnName("nodes")
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<List<GraphNode>>(v, jsonOptions) ?? new List<GraphNode>())
            .HasColumnType("TEXT");
        
        builder.Property(g => g.Edges)
            .HasColumnName("edges")
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<List<GraphEdge>>(v, jsonOptions) ?? new List<GraphEdge>())
            .HasColumnType("TEXT");

        // NOTE: not necessary for now. will require additional handling using concurrency exception
        // builder.Property<uint>("Version").IsRowVersion();
    }
}
