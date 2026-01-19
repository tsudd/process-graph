namespace ProcessGraph.API.Requests;

public record CreateProcessRequest(
    string Name,
    string? Description
);