using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Graphs;

namespace ProcessGraph.Domain.Processes;

public sealed class Process : Entity
{
    private Process()
    {
        
    }

    private Process(Guid id, string name, string description, ProcessSettings settings, ProcessStatus status,
        Graph graph)
    {
        Name = name;
        Description = description;
        Settings = settings;
        Status = status;
        Graph = graph;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProcessSettings Settings { get; private set; }
    public ProcessStatus Status { get; private set; }
    public Graph Graph { get; private set; }
    public ProcessTotals Totals => Graph.GetTotals(Settings);

    public static Process Create(
        string name,
        string description,
        ProcessSettings settings
    )
    {
        return new Process(Guid.NewGuid(), name, description, settings, ProcessStatus.NotStarted, Graph.CreateEmpty());
    }

    public void UpdateProcess(string name, string description, ProcessSettings settings)
    {
        Name = name;
        Description = description;
        Settings = settings;
        // TODO: use datetime provider?
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateGraph(IReadOnlyList<GraphNode> nodes, IReadOnlyList<GraphEdge> edges)
    {
        Graph.UpdateNodes(nodes);
        Graph.UpdateEdges(edges);
        LastModifiedAt = DateTime.UtcNow;
    }
}