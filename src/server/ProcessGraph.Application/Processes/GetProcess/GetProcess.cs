using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline;
using ProcessGraph.Application.Processes.Dtos;
using ProcessGraph.Domain.Processes;

namespace ProcessGraph.Application.Processes.GetProcess;

public sealed record GetProcess(Guid Id) : IRequest<Result<GetProcessDto>>;

public sealed class GetProcessHandler(IProcessRepository processRepository)
    : IRequestHandler<GetProcess, Result<GetProcessDto>>
{
    public async Task<Result<GetProcessDto>> HandleAsync(GetProcess request,
        CancellationToken cancellationToken = default)
    {
        var getProcess = await processRepository.GetByIdAsync(request.Id, cancellationToken);
        if (getProcess.IsFailed)
        {
            return getProcess.ToResult<GetProcessDto>();
        }

        var process = getProcess.Value;
        // TODO: Map using AutoMapper
        var dto = new GetProcessDto(
            process.Id,
            process.Name,
            process.Description,
            new ProcessSettingsDto(process.Settings.Unit.ToString()),
            process.Status,
            process.CreatedAt,
            process.Graph);
        return dto.ToResult();
    }
}