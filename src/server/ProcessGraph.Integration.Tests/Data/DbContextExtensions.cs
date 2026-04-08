using ProcessGraph.Domain.Processes;
using ProcessGraph.Infrastructure.Context;

namespace ProcessGraph.Integration.Tests.Data;

public static class DbContextExtensions
{
    /// <summary>
    /// Creates a Process using ProcessBuilder, applies the configure function, saves it to the context, and returns it.
    /// </summary>
    public static async Task<Process> AddProcessAsync(
        this ProcessGraphDbContext context,
        Func<ProcessBuilder, ProcessBuilder>? configure = null)
    {
        var builder = new ProcessBuilder();
        
        if (configure != null)
        {
            builder = configure(builder);
        }

        var process = builder.Build();
        
        context.Set<Process>().Add(process);
        await context.SaveChangesAsync();
        
        return process;
    }
}