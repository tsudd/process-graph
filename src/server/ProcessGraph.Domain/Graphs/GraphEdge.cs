using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Domain.Graphs;

public sealed record GraphEdge(
    Guid From,
    Guid To,
    Measure Value,
    string? Description = null,
    string? Label = null);