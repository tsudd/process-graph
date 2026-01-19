using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Application.Abstractions;
using ProcessGraph.Application.Abstractions.Pipeline;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Processes.CreateProcess;
using ProcessGraph.Application.Processes.GetProcess;

namespace ProcessGraph.Application.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRequestPipeline<,>), typeof(RequestPipeline<,>));
        services.AddScoped<ICommandHandler<CreateProcessCommand, Guid>, CreateProcessCommandHandler>();
        services.AddScoped<IQueryHandler<GetProcessQuery, ProcessResponse>, GetProcessQueryHandler>();

        return services;
    }
}