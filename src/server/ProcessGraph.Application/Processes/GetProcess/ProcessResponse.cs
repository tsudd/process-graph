using ProcessGraph.Application.Models;

namespace ProcessGraph.Application.Processes.GetProcess;

public sealed class ProcessResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProcessSettingsModel ProcessSettings { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public GraphModel Graph { get; set; }
}