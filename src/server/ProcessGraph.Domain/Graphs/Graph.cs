using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Domain.Graphs;

public sealed class Graph
{
    private Graph()
    {
    }

    public Graph(IList<GraphNode> nodes, IList<GraphEdge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public static Graph CreateEmpty()
    {
        return new Graph(new List<GraphNode>(), new List<GraphEdge>());
    }

    public void UpdateNodes(IReadOnlyList<GraphNode> nodes)
    {
        Nodes.Clear();
        foreach (var node in nodes)
        {
            Nodes.Add(node);
        }
    }

    public void UpdateEdges(IReadOnlyList<GraphEdge> edges)
    {
        Edges.Clear();
        foreach (var edge in edges)
        {
            Edges.Add(edge);
        }
    }

    public ProcessTotals GetTotals(ProcessSettings processSettings)
    {
        // Implementation omitted for brevity
        throw new NotImplementedException();
    }

    public IList<GraphNode> Nodes { get; init; }
    public IList<GraphEdge> Edges { get; init; }
}