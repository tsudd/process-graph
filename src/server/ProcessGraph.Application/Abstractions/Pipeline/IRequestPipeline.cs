using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline;

public interface IRequestPipeline<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> ExecuteAsync(
        TRequest request,
        CancellationToken cancellationToken = default
    );
}
