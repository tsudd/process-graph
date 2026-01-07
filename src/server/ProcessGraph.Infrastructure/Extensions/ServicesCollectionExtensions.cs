using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Infrastructure.Context;
using ProcessGraph.Infrastructure.Repositories;

namespace ProcessGraph.Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB") ??
                               throw new InvalidOperationException("MongoDB connection string is not configured.");
        var databaseName = configuration["MongoDB:DatabaseName"] ??
                           throw new InvalidOperationException("Database name is not configured.");

        services.AddDbContext<ProcessGraphDbContext>(options =>
        {
            options.UseMongoDB(connectionString, databaseName);
        });

        services.AddScoped<IProcessRepository, ProcessRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProcessGraphDbContext>());

        return services;
    }
}