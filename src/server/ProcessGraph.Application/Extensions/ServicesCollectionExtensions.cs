using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Application.Abstractions;
using ProcessGraph.Application.Abstractions.Pipeline;
using ProcessGraph.Application.Processes.CreateProcess;

namespace ProcessGraph.Application.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRequestPipeline<,>), typeof(RequestPipeline<,>));
        services.AddScoped<IRequestHandler<CreateProcess, Result<Guid>>, CreateProcessHandler>();

        return services;
    }
}