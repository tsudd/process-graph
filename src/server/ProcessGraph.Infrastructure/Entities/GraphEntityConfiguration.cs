using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProcessGraph.Domain.Graphs;

namespace ProcessGraph.Infrastructure.Entities;

internal sealed class GraphEntityConfiguration : IEntityTypeConfiguration<Graph>
{
    public void Configure(EntityTypeBuilder<Graph> builder)
    {
        // Configure the primary key for MongoDB
        builder.HasKey(g => g.Id);
        
        // MongoDB.EntityFrameworkCore will automatically handle:
        // - Collection name (defaults to "Graph" or can be configured via attributes)
        // - Document structure for embedded collections (Nodes and Edges)
        // - Field mapping (_id for Id, etc.)
        
        // The MongoDB provider will automatically serialize complex properties
        // like IList<GraphNode> and IList<GraphEdge> as embedded documents
    }
}
