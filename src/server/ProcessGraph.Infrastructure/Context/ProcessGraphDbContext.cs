using Microsoft.EntityFrameworkCore;
using ProcessGraph.Domain.Abstractions;

namespace ProcessGraph.Infrastructure.Context;

public sealed class ProcessGraphDbContext(DbContextOptions<ProcessGraphDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessGraphDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}