using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Models;
using ProcessGraph.Application.Processes.Dtos;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Application.Processes.UpdateProcess;

public sealed record UpdateProcess(Guid Id, string? Name, string? Description, ProcessSettingsModel? ProcessSettings)
    : ICommand;

public sealed class UpdateProcessHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProcess>
{
    public async Task<Result> HandleAsync(UpdateProcess request, CancellationToken cancellationToken = default)
    {
        var process = await processRepository.GetByIdAsync(request.Id, cancellationToken);

        if (process == null)
        {
            return Result.Fail($"Process with Id {request.Id} not found.");
        }

        process.UpdateProcess(request.Name ?? process.Name,
            request.Description ?? process.Description,
            request.ProcessSettings != null
                ? new ProcessSettings(UnitOfMeasure.FromName(request.ProcessSettings.Unit))
                : process.Settings);

        processRepository.Update(process);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}