using FluentValidation;

namespace ProcessGraph.Application.Processes.CreateProcess;

public class CreateProcessCommandValidator : AbstractValidator<CreateProcessCommand>
{
    public CreateProcessCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Process name is required.")
            .MaximumLength(Constants.ProcessNameMaxLength)
            .WithMessage($"Process name must not exceed {Constants.ProcessNameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(Constants.ProcessDescriptionMaxLength).WithMessage(
                $"Process description must not exceed {Constants.ProcessDescriptionMaxLength} characters.");
    }
}