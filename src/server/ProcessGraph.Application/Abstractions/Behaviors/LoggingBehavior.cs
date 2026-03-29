using Mediator;
using Microsoft.Extensions.Logging;

namespace ProcessGraph.Application.Abstractions.Behaviors;

public class LoggingBehavior<TMessage, TResponse>(ILogger<TMessage> logger) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IBaseCommand
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = message.GetType().Name;

        try
        {
            logger.LogInformation("Processing command {Command}", name);
            var response = await next(message, cancellationToken);
            logger.LogInformation("Command {Command} processed successfully", name);

            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Command {Command} processing failed", name);
            throw;
        }
    }
}