using System.Text.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProcessGraph.Application.Abstractions.Data;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Infrastructure.Context;
using ProcessGraph.Infrastructure.Data;
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
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        SqlMapper.AddTypeHandler(new GraphModelTypeHandler());

        return services;
    }
}