using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProcessGraph.Infrastructure.Context;

/// <summary>
/// Design-time factory for creating ProcessGraphDbContext instances.
/// This is used by EF Core tools for migrations and other design-time tasks.
/// </summary>
public class ProcessGraphDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ProcessGraphDbContext>
{
    public ProcessGraphDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ProcessGraphDbContext>()
            .UseMongoDB(
                "mongodb://localhost:27017",
                "ProcessGraphDb")
            .Options;

        return new ProcessGraphDbContext(options);
    }
}