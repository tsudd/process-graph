using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.CreateProcess;

public sealed record CreateProcess(string Name, string? Description) : ICommand<Result<Guid>>;

public sealed class CreateProcessHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProcess, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateProcess request, CancellationToken cancellationToken = default)
    {
        var newProcess = Process.Create(request.Name, string.Empty, ProcessSettings.CreateDefault());

        processRepository.Add(newProcess);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newProcess.Id.ToResult();
    }
}