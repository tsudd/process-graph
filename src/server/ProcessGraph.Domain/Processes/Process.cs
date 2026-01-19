using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Graphs;

namespace ProcessGraph.Domain.Processes;

public sealed class Process : Entity
{
    private Process()
    {
    }

    private Process(Guid id, string name, string description, ProcessSettings settings, ProcessStatus status,
        Graph graph, DateTime createdAt)
    {
        Name = name;
        Description = description;
        Settings = settings;
        Status = status;
        Graph = graph;
        Id = id;
        CreatedAt = createdAt;
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
        // TODO: use datetime provider?
        return new Process(Guid.NewGuid(), name, description, settings, ProcessStatus.NotStarted, Graph.CreateEmpty(),
            DateTime.UtcNow);
    }

    public void UpdateProcess(string name, string description, ProcessSettings settings)
    {
        Name = name;
        Description = description;
        Settings = settings;
        // TODO: use datetime provider?
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateGraph(Graph graph)
    {
        Graph = graph;
        LastModifiedAt = DateTime.UtcNow;
    }
}