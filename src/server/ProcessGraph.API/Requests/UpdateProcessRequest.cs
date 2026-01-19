using ProcessGraph.Application.Models;

namespace ProcessGraph.API.Requests;

public record UpdateProcessRequest(
    string Name,
    string? Description,
    ProcessSettingsModel? ProcessSettings
);