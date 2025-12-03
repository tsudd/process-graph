using System.Collections.Immutable;
using FluentResults;

namespace ProcessGraph.Domain.Processes;

public interface IProcessRepository
{
    Task<Result<Process>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(Process process, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Process process, CancellationToken cancellationToken = default);
    Task<Result<IImmutableList<Process>>> GetAllAsync(
        CancellationToken cancellationToken = default
    );
}
