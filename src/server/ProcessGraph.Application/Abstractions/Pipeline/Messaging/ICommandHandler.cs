using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline.Messaging;

public interface ICommandHandler<TCommand> where TCommand : IRequest<Result>
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand, TResponse> where TCommand : IRequest<Result<TResponse>>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}