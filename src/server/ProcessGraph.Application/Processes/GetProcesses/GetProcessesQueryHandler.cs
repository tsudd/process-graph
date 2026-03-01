using System.Text;
using Dapper;
using FluentResults;
using ProcessGraph.Application.Abstractions.Data;
using ProcessGraph.Application.Abstractions.Pipeline.Messaging;
using ProcessGraph.Application.Models;
using ProcessGraph.Application.Processes.GetProcess;
using ProcessGraph.Domain.Processes;
using ProcessGraph.Domain.Shared;

namespace ProcessGraph.Application.Processes.GetProcesses;

public sealed class GetProcessesQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProcessesQuery, PagedResult<ProcessResponse>>, Mediator.IRequestHandler<GetProcessesQuery, Result<PagedResult<ProcessResponse>>>
{
    public async Task<Result<PagedResult<ProcessResponse>>> HandleAsync(GetProcessesQuery query,
        CancellationToken cancellationToken = default)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        var (whereClause, parameters) = BuildWhereClause(query);
        var orderByClause = BuildOrderByClause(query.SortBy, query.SortDirection);
        
        // Main query for data
        var sql = $"""
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
                   {whereClause}
                   {orderByClause}
                   OFFSET @Offset LIMIT @Limit
                   """;

        // Count query for total
        var countSql = $"""
                        SELECT COUNT(*)
                        FROM processes p
                        {whereClause}
                        """;

        var offset = (query.Page - 1) * query.Limit;
        var queryParams = new DynamicParameters(parameters);
        queryParams.Add("@Offset", offset);
        queryParams.Add("@Limit", query.Limit);

        // Execute both queries
        var totalCountTask = connection.QuerySingleAsync<int>(countSql, parameters);
        
        var processesTask = connection.QueryAsync<ProcessResponse, ProcessSettingsModel, ProcessResponse>(
            sql,
            (proc, settings) =>
            {
                proc.ProcessSettings = settings;
                return proc;
            },
            queryParams,
            splitOn: "Unit");

        await Task.WhenAll(totalCountTask, processesTask);

        var totalCount = await totalCountTask;
        var processes = (await processesTask).ToList();

        var result = PagedResult<ProcessResponse>.Create(
            processes,
            totalCount,
            query.Page,
            query.Limit);

        return Result.Ok(result);
    }

    private static (string whereClause, object parameters) BuildWhereClause(GetProcessesQuery query)
    {
        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        // Search term filter using fuzzy search
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            conditions.Add("(p.name ILIKE @SearchPattern OR similarity(p.name, @SearchTerm) > 0.1)");
            parameters.Add("@SearchTerm", query.SearchTerm);
            parameters.Add("@SearchPattern", $"%{query.SearchTerm}%");
        }

        // Connection filter
        switch (query.ConnectionFilter)
        {
            case ProcessConnectionFilter.WithConnections:
                conditions.Add(@"EXISTS (
                    SELECT 1 
                    FROM jsonb_array_elements(p.graph->'nodes') AS node 
                    WHERE jsonb_array_length(COALESCE(node->'connections', '[]'::jsonb)) > 0
                )");
                break;
            case ProcessConnectionFilter.WithoutConnections:
                conditions.Add(@"NOT EXISTS (
                    SELECT 1 
                    FROM jsonb_array_elements(p.graph->'nodes') AS node 
                    WHERE jsonb_array_length(COALESCE(node->'connections', '[]'::jsonb)) > 0
                )");
                break;
            case ProcessConnectionFilter.Any:
            default:
                // No filtering
                break;
        }

        var whereClause = conditions.Count > 0 
            ? "WHERE " + string.Join(" AND ", conditions)
            : "";

        return (whereClause, parameters);
    }

    private static string BuildOrderByClause(ProcessSortBy sortBy, SortDirection sortDirection)
    {
        var direction = sortDirection == SortDirection.Ascending ? "ASC" : "DESC";
        
        return sortBy switch
        {
            ProcessSortBy.Name => $"ORDER BY p.name {direction}",
            ProcessSortBy.CreatedAt => $"ORDER BY p.created_at {direction}",
            _ => "ORDER BY p.created_at DESC"
        };
    }

    public async ValueTask<Result<PagedResult<ProcessResponse>>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }
}