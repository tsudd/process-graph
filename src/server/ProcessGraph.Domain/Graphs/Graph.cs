using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Domain.Graphs;

public sealed class Graph : Entity
{
    private Graph()
    {
    }

    public Graph(Guid Id, IList<GraphNode> Nodes, IList<GraphEdge> Edges)
    {
        this.Id = Id;
        this.Nodes = Nodes;
        this.Edges = Edges;
    }

    public static Graph CreateEmpty()
    {
        return new Graph(Guid.NewGuid(), new List<GraphNode>(), new List<GraphEdge>());
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

    public Guid Id { get; init; }
    public IList<GraphNode> Nodes { get; init; }
    public IList<GraphEdge> Edges { get; init; }
}