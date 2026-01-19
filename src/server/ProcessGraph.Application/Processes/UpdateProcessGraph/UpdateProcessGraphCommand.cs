using System.Collections.Immutable;
using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Models;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Graphs;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.UpdateProcessGraph;

public sealed record UpdateProcessGraphCommand(Guid Id, GraphModel Graph) : ICommand;

public sealed class UpdateProcessGraphCommandHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProcessGraphCommand>
{
    public async Task<Result> HandleAsync(UpdateProcessGraphCommand command,
        CancellationToken cancellationToken = default)
    {
        var process = await processRepository.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);

        if (process == null)
        {
            return Result.Fail($"Process with Id {command.Id} not found.");
        }

        var graphNodes = command.Graph.Nodes
            .Select(n => new GraphNode(n.Id, n.Label, n.XPosition, n.YPosition, n.Description))
            .ToImmutableList();

        var graphEdges = command.Graph.Edges
            .Select(e => new GraphEdge(e.From, e.To, e.Value))
            .ToImmutableList();

        var newGraph = new Graph(graphNodes, graphEdges);

        process.UpdateGraph(newGraph);

        processRepository.Update(process);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok();
    }
}