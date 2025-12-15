namespace ProcessGraph.Application.Processes.Dtos;

public sealed record GraphEdgeDto(
    Guid From,
    Guid To,
    int Value,
    string UnitOfMeasure,
    string? Description = null,
    string? Label = null);