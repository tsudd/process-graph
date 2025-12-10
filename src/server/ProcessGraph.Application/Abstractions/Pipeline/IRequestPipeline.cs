namespace ProcessGraph.Application.Abstractions.Pipeline;

public interface IRequestPipeline
{
    Task<TResponse> ExecuteAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    );
}
