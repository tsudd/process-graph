using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest command, CancellationToken cancellationToken = default);
}
