namespace ProcessGraph.Domain.Processes;

public record GraphNode(Guid Id, string Label, int XPosition, int YPosition, string Description, string? Unit);