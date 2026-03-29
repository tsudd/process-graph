using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProcessGraph.Application.Abstractions.Behaviors;

namespace ProcessGraph.Application.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.Assemblies = [typeof(ServicesCollectionExtensions).Assembly];
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(LoggingBehavior<,>), typeof(ValidationBehavior<,>)];
        });
        
        services.AddValidatorsFromAssembly(typeof(ServicesCollectionExtensions).Assembly);

        return services;
    }
}