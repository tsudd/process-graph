using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Mediator;

namespace ProcessGraph.Application.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        return services;
    }
}