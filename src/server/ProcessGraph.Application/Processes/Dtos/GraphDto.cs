namespace ProcessGraph.Application.Processes.Dtos;

public sealed record GraphDto(IEnumerable<GraphNodeDto> Nodes, IEnumerable<GraphEdgeDto> Edges)
{
    
}