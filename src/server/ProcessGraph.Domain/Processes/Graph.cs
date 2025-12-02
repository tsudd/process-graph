namespace ProcessGraph.Domain.Processes;

public record Graph(IList<GraphNode> Nodes, IList<GraphEdge> Edges);

