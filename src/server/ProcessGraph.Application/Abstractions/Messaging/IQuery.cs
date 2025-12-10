using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>;
