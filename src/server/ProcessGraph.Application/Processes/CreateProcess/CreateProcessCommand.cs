using FluentResults;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.CreateProcess;

public sealed record CreateProcessCommand(string Name, string? Description) : Mediator.ICommand<Result<Guid>>;

public sealed class CreateProcessCommandHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : Mediator.ICommandHandler<CreateProcessCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateProcessCommand command, CancellationToken cancellationToken)
    {
        var newProcess = Process.Create(command.Name, command.Description ?? string.Empty,
            ProcessSettings.CreateDefault());

        processRepository.Add(newProcess);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newProcess.Id.ToResult();
    }
}