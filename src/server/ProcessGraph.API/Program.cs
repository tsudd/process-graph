using ProcessGraph.Infrastructure.Extensions;
using ProcessGraph.API.Endpoints;
using ProcessGraph.API.Extensions;
using ProcessGraph.API.Middleware;
using ProcessGraph.Application.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapApiEndpoints();

app.Run();

/// <summary>
/// Exposed for tests
/// </summary>
public partial class Program;

