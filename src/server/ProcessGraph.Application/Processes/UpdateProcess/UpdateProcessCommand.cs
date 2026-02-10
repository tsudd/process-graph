using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Models;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Application.Processes.UpdateProcess;

public sealed record UpdateProcessCommand(
    Guid Id,
    string? Name,
    string? Description,
    ProcessSettingsModel? ProcessSettings)
    : ICommand, Mediator.IRequest<Result>;

public sealed class UpdateProcessCommandHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProcessCommand>, Mediator.IRequestHandler<UpdateProcessCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateProcessCommand command,
        CancellationToken cancellationToken = default)
    {
        var process = await processRepository.GetByIdAsync(command.Id, cancellationToken);

        if (process == null)
        {
            return Result.Fail($"Process with Id {command.Id} not found.");
        }

        process.UpdateProcess(command.Name ?? process.Name,
            command.Description ?? process.Description,
            command.ProcessSettings != null
                ? new ProcessSettings(UnitOfMeasure.FromName(command.ProcessSettings.Unit))
                : process.Settings);

        processRepository.Update(process);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async ValueTask<Result> Handle(UpdateProcessCommand request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }
}