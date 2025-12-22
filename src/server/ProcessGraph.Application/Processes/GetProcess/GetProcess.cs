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
        var process = await processRepository.GetByIdAsync(request.Id, cancellationToken);
        if (process == null)
        {
            return Result.Fail($"Process with ID {request.Id} not found.");
        }

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