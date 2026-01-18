using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;