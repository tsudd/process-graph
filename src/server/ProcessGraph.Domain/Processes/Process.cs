using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Graphs;

namespace ProcessGraph.Domain.Processes;

public sealed class Process : Entity
{
    private Process(Guid id, string name, string description, ProcessSettings settings, ProcessStatus status,
        Graph graph, ProcessTotals totals)
    {
        Name = name;
        Description = description;
        Settings = settings;
        Status = status;
        Graph = graph;
        Totals = totals;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProcessSettings Settings { get; private set; }
    public ProcessStatus Status { get; private set; }
    public Graph Graph { get; private set; }
    public ProcessTotals Totals { get; private set; }

    public static Process Create(
        string name,
        string description,
        ProcessSettings settings
    )
    {
        return new Process(Guid.NewGuid(), name, description, settings, ProcessStatus.NotStarted, Graph.CreateEmpty(),
            ProcessTotals.GetDefault(settings.Unit));
    }
}