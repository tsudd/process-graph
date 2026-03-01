using FluentResults;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Models;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Application.Processes.GetProcesses;

public sealed record GetProcessesQuery(
    string? SearchTerm = null,
    ProcessConnectionFilter ConnectionFilter = ProcessConnectionFilter.Any,
    ProcessSortBy SortBy = ProcessSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending,
    int Page = 1,
    int Limit = 20
) : IQuery<PagedResult<ProcessResponse>>, Mediator.IRequest<Result<PagedResult<ProcessResponse>>>;