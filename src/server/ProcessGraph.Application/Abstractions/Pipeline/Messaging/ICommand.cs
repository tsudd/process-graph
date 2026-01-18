using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<out TResponse> : IRequest<TResponse>, IBaseCommand;

public interface IBaseCommand;