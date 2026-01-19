using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.DeleteProcess;

public sealed record DeleteProcessCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteProcessCommandHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProcessCommand>
{
    public async Task<Result> HandleAsync(DeleteProcessCommand command, CancellationToken cancellationToken = default)
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