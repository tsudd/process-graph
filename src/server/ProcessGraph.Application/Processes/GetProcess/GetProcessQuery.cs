using Dapper;
using FluentResults;
using Mediator;
using ProcessGraph.Application.Abstractions.Data;
using ProcessGraph.Application.Models;

namespace ProcessGraph.Application.Processes.GetProcess;

public sealed record GetProcessQuery(Guid Id) : IQuery<Result<ProcessResponse>>;

public sealed class GetProcessQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProcessQuery, Result<ProcessResponse>>
{
    public async ValueTask<Result<ProcessResponse>> Handle(GetProcessQuery query, CancellationToken cancellationToken)
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
                ProcessId = query.Id
            },
            splitOn: "Unit").ConfigureAwait(false);

        var processDto = getQuery.FirstOrDefault();

        if (processDto == null)
            return Result.Fail("Process not found.");

        return processDto;
    }
}