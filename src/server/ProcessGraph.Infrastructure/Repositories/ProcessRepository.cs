using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Infrastructure.Context;

namespace ProcessGraph.Infrastructure.Repositories;

internal sealed class ProcessRepository(ProcessGraphDbContext context) : IProcessRepository
{
    public async Task<Process?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<Process>()
            .Include(process => process.Graph)
            .FirstOrDefaultAsync(process => process.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public void Add(Process process)
    {
        context.Set<Process>().Add(process);
    }

    public void Update(Process process)
    {
        context.Update(process);
        
        var graphEntry = context.Entry(process.Graph);
        if (graphEntry.State == EntityState.Modified || graphEntry.State == EntityState.Unchanged)
        {
            graphEntry.Property(g => g.Nodes).IsModified = true;
            graphEntry.Property(g => g.Edges).IsModified = true;
        }
    }

    public void Delete(Process process)
    {
        context.Set<Process>().Remove(process);
    }

    public async Task<IImmutableList<Process>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}