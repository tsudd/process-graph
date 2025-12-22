using System.Collections.Immutable;
using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;
using ProcessGraph.Application.Processes.Dtos;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Graphs;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Application.Processes.UpdateProcessGraph;

public sealed record UpdateProcessGraph(Guid Id, GraphDto Graph) : IRequest<Result>;

public sealed class UpdateProcessGraphHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProcessGraph, Result>
{
    public async Task<Result> HandleAsync(UpdateProcessGraph request, CancellationToken cancellationToken = default)
    {
        var process = await processRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (process == null)
        {
            return Result.Fail($"Process with Id {request.Id} not found.");
        }

        // TODO: apply some mappers
        process.UpdateGraph(
            request.Graph.Nodes.Select(n => new GraphNode(n.Id, n.Label, n.XPosition, n.YPosition, n.Description))
                .ToImmutableList(),
            request.Graph.Edges.Select(e =>
                    new GraphEdge(e.From, e.To, new Measure(e.Value, UnitOfMeasure.FromName(e.UnitOfMeasure))))
                .ToImmutableList());
        
        processRepository.Update(process);
        
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok();
    }
}