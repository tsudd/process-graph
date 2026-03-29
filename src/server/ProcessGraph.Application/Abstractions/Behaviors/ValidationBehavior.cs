using FluentValidation;
using Mediator;
using ProcessGraph.Application.Exceptions;

namespace ProcessGraph.Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IMediator mediator, IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async ValueTask<TResponse> Handle(TRequest message, MessageHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(message, cancellationToken);
        }

        var context = new ValidationContext<TRequest>(message);

        var validationErrors = validators.Select(validator => validator.Validate(context))
            .Where(static validationResult => validationResult.Errors.Any())
            .SelectMany(result => result.Errors)
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToList();

        if (validationErrors.Any())
        {
            throw new Exceptions.ValidationException(validationErrors);
        }

        return await next(message, cancellationToken);
    }
}