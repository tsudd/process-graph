namespace ProcessGraph.Domain.Processes;

public record Graph(IList<GraphNode> Nodes, IList<GraphEdge> Edges)
{
    public static Graph Empty { get; } = new Graph(Array.Empty<GraphNode>(), Array.Empty<GraphEdge>());
}

