using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;
using ProcessGraph.Domain;
using ProcessGraph.Domain.Graphs;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.CreateProcess;

public record CreateProcess(string Name, string? Description) : IRequest<Result<Guid>>;

public sealed class CreateProcessHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProcess, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(CreateProcess request, CancellationToken cancellationToken = default)
    {
        var newProcess = Process.Create(request.Name, string.Empty, ProcessSettings.CreateDefault());

        processRepository.Add(newProcess);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return newProcess.Id.ToResult();
    }
}