using Microsoft.EntityFrameworkCore;
using ProcessGraph.Domain.Abstractions;

namespace ProcessGraph.Infrastructure.Context;

public sealed class ProcessGraphDbContext(DbContextOptions<ProcessGraphDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enable PostgreSQL pg_trgm extension for fuzzy search
        modelBuilder.HasPostgresExtension("pg_trgm");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProcessGraphDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}