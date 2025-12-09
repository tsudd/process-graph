using ProcessGraph.Domain.Abstractions;

namespace ProcessGraph.Domain.Processes;

public sealed class Process : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ProcessSettings Settings { get; private set; }
    public ProcessStatus Status { get; private set; }
    public Graph Graph { get; private set; }
    public ProcessTotals Totals { get; private set; }

    public static Process Create(
        string name,
        string description,
        ProcessSettings settings,
        Graph graph
    )
    {
        return new Process
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = name,
            Description = description,
            Settings = settings,
            Status = ProcessStatus.NotStarted,
            Graph = graph,
            Totals = ProcessTotals.GetDefault(settings.Unit),
        };
    }
}
