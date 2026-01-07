namespace ProcessGraph.Application.Processes.Dtos;

public record CreateProcessRequest(
    string Name,
    string? Description
);

public record UpdateProcessRequest(
    Guid Id,
    string Name,
    string? Description
);
