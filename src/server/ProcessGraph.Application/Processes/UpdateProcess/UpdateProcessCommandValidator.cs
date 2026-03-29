using FluentValidation;

namespace ProcessGraph.Application.Processes.UpdateProcess;

public class UpdateProcessCommandValidator : AbstractValidator<UpdateProcessCommand>
{
    public UpdateProcessCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(Constants.ProcessNameMaxLength)
            .When(x => x != null).WithMessage($"Process name must not exceed {Constants.ProcessNameMaxLength} characters.")
            .NotEmpty().WithMessage("Process name must not be empty.");
        

        RuleFor(x => x.Description)
            .MaximumLength(Constants.ProcessDescriptionMaxLength)
            .When(x => x != null)
            .WithMessage($"Process description must not exceed {Constants.ProcessDescriptionMaxLength} characters.");
    }
}