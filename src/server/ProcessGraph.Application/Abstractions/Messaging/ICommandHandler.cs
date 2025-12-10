using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Abstractions.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : IRequest<Result>, IBaseCommand;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>, IBaseCommand;
