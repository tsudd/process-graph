namespace ProcessGraph.Domain.Graphs;

public sealed record GraphEdge(
    Guid From,
    Guid To,
    int Value,
    string? Description = null,
    string? Label = null);