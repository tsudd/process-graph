using FluentResults;
using Mediator;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.DeleteProcess;

public sealed record DeleteProcessCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteProcessCommandHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProcessCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteProcessCommand command, CancellationToken cancellationToken)
    {
        var process = await processRepository.GetByIdAsync(command.Id, cancellationToken);

        if (process == null)
        {
            return Result.Fail($"Process with Id {command.Id} not found.");
        }

        processRepository.Delete(process);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Ok();
    }
}