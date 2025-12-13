namespace ProcessGraph.Domain.Graphs;

public sealed record Graph(Guid Id, IList<GraphNode> Nodes, IList<GraphEdge> Edges)
{
    public Guid Id { get; } = Id;
    public IList<GraphNode> Nodes { get; } = Nodes;
    public IList<GraphEdge> Edges { get; } = Edges;
    
    public static Graph CreateEmpty()
    {
        return new Graph(Guid.NewGuid(), new List<GraphNode>(), new List<GraphEdge>());
    }
}