using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Abstractions;

internal sealed class RequestPipeline<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
    : IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResultBase
{
    public Task<TResponse> ExecuteAsync(TRequest request,
        CancellationToken cancellationToken = default)
    {
        return handler.HandleAsync(request, cancellationToken);
    }
}