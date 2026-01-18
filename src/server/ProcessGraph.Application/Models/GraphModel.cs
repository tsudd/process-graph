namespace ProcessGraph.Application.Models;

using System;
using System.Collections.Generic;

public class GraphModel
{
    // Parameterless constructor and mutable collections make this class friendly to Dapper and model binding
    public GraphModel()
    {
        Nodes = new List<GraphNodeModel>();
        Edges = new List<GraphEdgeModel>();
    }

    public static GraphModel CreateEmpty() => new GraphModel();

    public IList<GraphNodeModel> Nodes { get; set; }
    public IList<GraphEdgeModel> Edges { get; set; }
}

public class GraphNodeModel
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int XPosition { get; set; }
    public int YPosition { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class GraphEdgeModel
{
    public Guid From { get; set; }
    public Guid To { get; set; }
    public int Value { get; set; }
    public string? Description { get; set; }
    public string? Label { get; set; }
}