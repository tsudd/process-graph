namespace ProcessGraph.Domain.Processes;

public record GraphEdge(Guid From, Guid To, Measure Value, string? Description = null, string? Label = null);