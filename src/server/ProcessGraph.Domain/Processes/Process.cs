using ProcessGraph.Domain.Abstractions;

namespace ProcessGraph.Domain.Processes;

public sealed class Process(
    string name,
    string description,
    ProcessSettings settings,
    DateTime createdAt,
    DateTime lastModifiedAt,
    Graph graph,
    ProcessTotals totals,
    ProcessStatus status
) : Entity
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
    public ProcessSettings Settings { get; private set; } = settings;
    public ProcessStatus Status { get; private set; } = status;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public DateTime LastModifiedAt { get; private set; } = lastModifiedAt;
    public Graph Graph { get; private set; } = graph;
    public ProcessTotals Totals { get; private set; } = totals;
}

