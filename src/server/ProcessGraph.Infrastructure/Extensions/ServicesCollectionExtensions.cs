using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Infrastructure.Context;
using ProcessGraph.Infrastructure.Repositories;

namespace ProcessGraph.Infrastructure.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new ArgumentException("Database connection string is required");

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        dataSourceBuilder.ConfigureJsonOptions(jsonOptions);
        var dataSource = dataSourceBuilder.Build();
        services.AddDbContext<ProcessGraphDbContext>(options =>
        {
            options.UseNpgsql(dataSource).UseSnakeCaseNamingConvention();
        });
        services.AddScoped<IProcessRepository, ProcessRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ProcessGraphDbContext>());

        return services;
    }
}