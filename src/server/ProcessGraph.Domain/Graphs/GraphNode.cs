namespace ProcessGraph.Domain.Graphs;

public sealed record GraphNode(Guid Id, string Label, int XPosition, int YPosition, string Description);