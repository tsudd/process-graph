using Microsoft.EntityFrameworkCore;
using ProcessGraph.Domain.Abstractions;

namespace ProcessGraph.Infrastructure;

public sealed class ProcessGraphDbContext(DbContextOptions options) : DbContext, IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessGraphDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}