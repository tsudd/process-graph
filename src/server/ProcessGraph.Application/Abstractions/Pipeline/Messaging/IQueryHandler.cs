using FluentResults;

namespace ProcessGraph.Application.Abstractions.Pipeline.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}