using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Infrastructure.Repositories;

namespace ProcessGraph.Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Infrastructure services registration goes here
        services.AddDbContext<ProcessGraphDbContext>(options => { options.UseSqlite("Data Source=process.graph.db"); });
        services.AddScoped<IProcessRepository, ProcessRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProcessGraphDbContext>());

        return services;
    }
}