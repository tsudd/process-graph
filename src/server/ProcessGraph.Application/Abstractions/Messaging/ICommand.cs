using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Abstractions.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface IBaseCommand;
