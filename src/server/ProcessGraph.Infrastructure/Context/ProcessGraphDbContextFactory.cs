using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProcessGraph.Infrastructure.Context;

public sealed class ProcessGraphDbContextFactory : IDesignTimeDbContextFactory<ProcessGraphDbContext>
{
    public ProcessGraphDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProcessGraphDbContext>();

        // Default connection string for migrations - should be overridden in production
        var connectionString = "Host=localhost;Database=ProcessGraphDb;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString,
            options => { options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery); });

        // Use snake_case naming convention for PostgreSQL
        optionsBuilder.UseSnakeCaseNamingConvention();

        return new ProcessGraphDbContext(optionsBuilder.Options);
    }
}