using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Abstractions.Messaging;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : IRequest<Result>, IBaseCommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>, IBaseCommand;
