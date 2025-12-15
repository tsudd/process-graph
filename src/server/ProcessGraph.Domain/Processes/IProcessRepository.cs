using System.Collections.Immutable;
using FluentResults;

namespace ProcessGraph.Domain.Processes;

public interface IProcessRepository
{
    Task<Result<Process>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Result Add(Process process);
    Result Update(Process process);
    Result Delete(Process process);
    Task<Result<IImmutableList<Process>>> GetAllAsync(CancellationToken cancellationToken = default);
}