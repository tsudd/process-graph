using System.Collections.Immutable;
using FluentResults;

namespace ProcessGraph.Domain.Processes;

public interface IProcessRepository
{
    Task<Process?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Process process);
    void Update(Process process);
    void Delete(Process process);
    Task<IImmutableList<Process>> GetAllAsync(CancellationToken cancellationToken = default);
}