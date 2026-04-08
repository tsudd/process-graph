using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using ProcessGraph.Application.Abstractions.Data;
using ProcessGraph.Infrastructure.Context;
using ProcessGraph.Infrastructure.Data;
using Testcontainers.PostgreSql;
using Xunit;

namespace ProcessGraph.Integration.Tests.Infrastructure;

public class ProcessGraphTestApp : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:latest")
        .WithDatabase("processgraph")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // database access for CRUD
            services.RemoveAll(typeof(DbContextOptions<ProcessGraphDbContext>));
            // TODO: duplications?
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_dbContainer.GetConnectionString());
            dataSourceBuilder.EnableDynamicJson();
            dataSourceBuilder.ConfigureJsonOptions(jsonOptions);
            var dataSource = dataSourceBuilder.Build();
            services.AddDbContext<ProcessGraphDbContext>(options =>
            {
                options.UseNpgsql(dataSource).UseSnakeCaseNamingConvention();
            });

            // database access for Dapper
            services.RemoveAll(typeof(ISqlConnectionFactory));
            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(_dbContainer.GetConnectionString()));
        });
    }

    public IServiceScope GetScope()
    {
        return Services.CreateScope();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.StopAsync();
    }
}