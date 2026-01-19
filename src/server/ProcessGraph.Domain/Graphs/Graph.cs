using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Domain.Graphs;

public sealed class Graph
{
    public static Graph CreateEmpty()
    {
        return new Graph(new List<GraphNode>(), new List<GraphEdge>());
    }

    private Graph()
    {
    }

    public Graph(IList<GraphNode> nodes, IList<GraphEdge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public ProcessTotals GetTotals(ProcessSettings processSettings)
    {
        // Implementation omitted for brevity
        throw new NotImplementedException();
    }

    public IList<GraphNode> Nodes { get; init; }
    public IList<GraphEdge> Edges { get; init; }
}