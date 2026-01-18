using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> where TCommand : IRequest<Result>;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : IRequest<Result<TResponse>>;