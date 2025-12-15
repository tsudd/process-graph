using ProcessGraph.Application.Processes.Dtos;
using ProcessGraph.Domain.Graphs;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.GetProcess;

public record GetProcessDto(
    Guid Id,
    string Name,
    string Description,
    ProcessSettingsDto ProcessSettings,
    ProcessStatus Status,
    DateTime CreatedAt,
    Graph Graph
);