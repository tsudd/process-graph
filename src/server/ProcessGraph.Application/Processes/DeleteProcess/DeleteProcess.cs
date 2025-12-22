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
        var process = await processRepository.GetByIdAsync(request.Id, cancellationToken);

        if (process == null)
        {
            return Result.Fail($"Process with Id {request.Id} not found.");
        }

        processRepository.Delete(process);

        return Result.Ok();
    }
}