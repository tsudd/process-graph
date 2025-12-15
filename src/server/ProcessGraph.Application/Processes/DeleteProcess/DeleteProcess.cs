using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;
using ProcessGraph.Domain.Abstractions;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.DeleteProcess;

public sealed record DeleteProcess(Guid Id) : IRequest<Result>;

public sealed class DeleteProcessHandler(IProcessRepository processRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProcess, Result>
{
    public async Task<Result> HandleAsync(DeleteProcess request, CancellationToken cancellationToken = default)
    {
        var getProcess = await processRepository.GetByIdAsync(request.Id, cancellationToken);

        if (getProcess.IsFailed)
        {
            return getProcess.ToResult();
        }

        var process = getProcess.Value;

        processRepository.Delete(process);

        return Result.Ok();
    }
}