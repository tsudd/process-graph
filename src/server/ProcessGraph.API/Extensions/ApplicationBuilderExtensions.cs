using Microsoft.EntityFrameworkCore;
using ProcessGraph.Infrastructure.Context;

namespace ProcessGraph.API.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// For local development only: Applies any pending migrations to the database
    /// </summary>
    /// <param name="app">Any application</param>
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProcessGraphDbContext>();
        dbContext.Database.Migrate();
    }
}