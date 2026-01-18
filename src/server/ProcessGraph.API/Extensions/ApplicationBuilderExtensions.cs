using Microsoft.EntityFrameworkCore;
using ProcessGraph.Infrastructure.Context;

namespace ProcessGraph.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProcessGraphDbContext>();
        dbContext.Database.Migrate();
    }
}