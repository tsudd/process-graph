using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;

namespace ProcessGraph.Application.Processes.CreateProcess;

public record CreateProcess : IRequest<Result<int>>
{
    public string Name { get; init; }
}

public sealed class CreateProcessHandler : IRequestHandler<CreateProcess, Result<int>>
{
    public Task<Result<int>> HandleAsync(CreateProcess request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}