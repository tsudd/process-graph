using Dapper;
using FluentResults;
using ProcessGraph.Application.Abstractions.Data;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Models;

namespace ProcessGraph.Application.Processes.GetProcess;

public sealed record GetProcessQuery(Guid Id) : IQuery<ProcessResponse>, Mediator.IRequest<Result<ProcessResponse>>;

public sealed class GetProcessQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProcessQuery, ProcessResponse>, Mediator.IRequestHandler<GetProcessQuery, Result<ProcessResponse>>
{
    public async Task<Result<ProcessResponse>> HandleAsync(GetProcessQuery command,
        CancellationToken cancellationToken = default)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
                                       SELECT 
                                           p.id as Id,
                                           p.name as Name,
                                           p.description as Description,
                                           p.status as Status,
                                           p.graph as Graph,
                                           p.created_at as CreatedAt,
                                           p.last_modified_at as LastModifiedAt,
                                           p.settings_unit AS Unit
                                       FROM processes p
                                       WHERE p.Id = @ProcessId
                           """;
        var getQuery = await connection.QueryAsync<ProcessResponse, ProcessSettingsModel, ProcessResponse>(
            sql,
            (proc, settings) =>
            {
                proc.ProcessSettings = settings;
                return proc;
            },
            new
            {
                ProcessId = command.Id
            },
            splitOn: "Unit").ConfigureAwait(false);

        var processDto = getQuery.FirstOrDefault();

        if (processDto == null)
            return Result.Fail("Process not found.");

        return processDto;
    }

    public async ValueTask<Result<ProcessResponse>> Handle(GetProcessQuery request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }
}