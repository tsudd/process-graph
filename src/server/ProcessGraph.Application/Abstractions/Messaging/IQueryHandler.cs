using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
